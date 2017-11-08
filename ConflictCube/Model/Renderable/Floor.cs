using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ConflictCube.Model.Renderable
{
    public class Floor : RenderableLayer
    {

        //Non - static
        public List<IMoveable> AttachedObjects { get; private set; }
        public Vector2 FloorTileSize;
        private Box2D _FloorBox;
        public Box2D FloorBox
        {
            get
            {
                return _FloorBox;
            }
            set
            {
                SetSizeTo(value, _FloorSize);
            }
        }
        private Vector2 _FloorSize;
        public Vector2 FloorSize
        {
            get
            {
                return _FloorSize;
            }
            set
            {
                SetSizeTo(_FloorBox, value);
            }
        }
        public FloorTile[,] FloorTiles { get; set; }

        private float TotalMovedDistanceDown = 0;

        public Floor(Vector2 floorSize, Box2D floorBox) : base(new List<RenderableObject>())
        {
            AttachedObjects = new List<IMoveable>();
            FloorTiles = new FloorTile[(int)floorSize.Y, (int)floorSize.X];
            SetSizeTo(floorBox, floorSize);
        }

        public void MoveFloorUp(float distance)
        {
            TotalMovedDistanceDown += distance;
            foreach (FloorTile floorTile in FloorTiles)
            {
                floorTile.Box.MinY -= distance;
            }


            foreach (IMoveable attachedObject in AttachedObjects)
            {
                attachedObject.SetPosition(new Vector2(attachedObject.GetPosition().X, attachedObject.GetPosition().Y - distance));
            }
        }


        public void AddFloorTile(FloorTile floorTile, int y, int x)
        {
            FloorTiles[y, x] = floorTile;
            ObjectsToRender.Add(floorTile);
        }

        public void AddAttachedObject(IMoveable moveable)
        {
            AttachedObjects.Add(moveable);
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
                        return new Vector2(tile.Box.CenterX, tile.Box.CenterY);
                    }
                }
            }

            // No tile in the whole level is of type floor....
            throw new Exception("No start position found");
        }

        public Box2D BoxInFloorGrid(float row, float column)
        {
            float posX = _FloorBox.MinX + column * FloorTileSize.X;
            float posY = _FloorBox.MinY + ((FloorTiles.GetLength(0) - 1) - row) * FloorTileSize.Y;

            return new Box2D(posX, posY, FloorTileSize.X, FloorTileSize.Y);
        }

        private void SetSizeTo(Box2D floorBox, Vector2 floorSize)
        {
            _FloorBox = floorBox;
            _FloorSize = floorSize;

            FloorTileSize.X = _FloorBox.SizeX * 2 / _FloorSize.X;
            FloorTileSize.Y = _FloorBox.SizeY * 2 / _FloorSize.Y;

            if (FloorTiles != null && FloorTiles.LongLength != 0)
            {
                foreach (FloorTile tile in FloorTiles)
                {
                    if (tile != null)
                    {
                        tile.Box = BoxInFloorGrid(tile.Row, tile.Column);
                    }
                }
            }
        }
    }
}
