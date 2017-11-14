using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ConflictCube.Model.Renderable
{
    public class Floor : RenderableLayer
    {
        public List<Tuple<IMoveable, Matrix3>> AttachedObjects { get; private set; }
        public Vector2 FloorTileSize;
        private Vector2 _FloorSize;
        private Vector2 FloorSize {
            get {
                return _FloorSize;
            }
            set {
                _FloorSize = value;

                FloorTileSize.X = 2 / _FloorSize.X;
                FloorTileSize.Y = 2 / _FloorSize.Y;
            }
        }
        public FloorTile[,] FloorTiles { get; set; }

        private float TotalMovedDistanceDown = 0;

        //floorSize is the size of the whole floor and not only the part which should be shown.
        public Floor(Vector2 floorSize, Box2D areaOfLayer) : base(new List<RenderableObject>(), new List<RenderableLayer>(), areaOfLayer)
        {
            AttachedObjects = new List<Tuple<IMoveable, Matrix3>>();
            FloorTiles = new FloorTile[(int)floorSize.Y, (int)floorSize.X];
            FloorSize = floorSize;
        }

        public void MoveFloorUp(float distance)
        {
            TotalMovedDistanceDown += distance;
            foreach (FloorTile floorTile in FloorTiles)
            {
                floorTile.Box.MinY -= distance;
            }


            foreach (Tuple<IMoveable, Matrix3> attachedObject in AttachedObjects)
            {
                Vector3 distVector = Vector3.Transform(new Vector3(0, distance, 1), attachedObject.Item2);
                attachedObject.Item1.Move(-distVector.Xy);
            }
        }


        public void AddFloorTile(FloorTile floorTile, int y, int x)
        {
            FloorTiles[y, x] = floorTile;
            ObjectsToRender.Add(floorTile);
        }

        public void AddAttachedObject(IMoveable moveable, Matrix3 scaleMatrix)
        {
            AttachedObjects.Add(new Tuple<IMoveable, Matrix3>(moveable, scaleMatrix));
        }

        public Vector2 FindStartPosition()
        {
            int TilesNotVisibleAnymore = (int)Math.Floor(TotalMovedDistanceDown / FloorTileSize.Y);
            int LowestRow = FloorTiles.GetLength(0) - TilesNotVisibleAnymore;
            int LowestRowIndex = LowestRow - 1;

            for (int i = LowestRowIndex; i >= 0; i--)
            {
                for (int u = 0; u < FloorTiles.GetLength(1); u++)
                {
                    FloorTile tile = FloorTiles[i, u];
                    if (tile.Type == TileType.Floor)
                    {
                        Vector2 startPos = new Vector2(tile.Box.CenterX, tile.Box.CenterY);
                        startPos = TransformPointToParent(startPos);
                        return startPos;
                    }
                }
            }

            // No tile in the whole level is of type floor....
            throw new Exception("No start position found");
        }

        public Box2D BoxInFloorGrid(float row, float column)
        {
            float posX = -1 + column * FloorTileSize.X;
            float posY = -1 + ((FloorTiles.GetLength(0) - 1) - row) * FloorTileSize.Y;

            return new Box2D(posX, posY, FloorTileSize.X, FloorTileSize.Y);
        }
    }
}
