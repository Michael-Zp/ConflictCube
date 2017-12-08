using OpenTK;
using System;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;

namespace ConflictCube.ComponentBased
{
    public class Floor : GameObject
    {
        public Vector2 FloorTileSize;
        private int _FloorRows;
        public int FloorRows {
            get {
                return _FloorRows;
            }
            set {
                _FloorRows = value;
                
                FloorTileSize.Y = 2 / _FloorRows;
            }
        }
        private int _FloorColumns;
        public int FloorColumns {
            get {
                return _FloorColumns;
            }
            set {
                _FloorColumns = value;

                FloorTileSize.X = 2 / _FloorColumns;
            }
        }

        public FloorTile[,] FloorTiles { get; set; }

        private float TotalMovedDistanceDown = 0;

        //floorSize is the size of the whole floor and not only the part which should be shown.
        public Floor(string name, Transform transform, GameObject parent, int rows, int columns) : base(name, transform, parent)
        {
            FloorTiles = new FloorTile[rows, columns];
            FloorRows = rows;
            FloorColumns = columns;
        }

        public void MoveFloorUp(float distance)
        {
            //Will be replaced.
        }


        public void AddFloorTile(FloorTile floorTile, int y, int x)
        {
            FloorTiles[y, x] = floorTile;
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
                    if (tile.Type == GameObjectType.Floor)
                    {
                        Vector2 startPos = new Vector2(tile.Transform.Position.X, tile.Transform.Position.Y);
                        startPos = Transform.TransformPointToParent(startPos);
                        return startPos;
                    }
                }
            }

            // No tile in the whole level is of type floor....
            throw new Exception("No start position found");
        }

        public Transform BoxInFloorGrid(float row, float column)
        {
            float posX = -1 + column * FloorTileSize.X;
            float posY = -1 + ((FloorTiles.GetLength(0) - 1) - row) * FloorTileSize.Y;

            return new Transform(posX, posY, FloorTileSize.X, FloorTileSize.Y);
        }

        public Vector2 GetGridPosition(Vector2 position)
        {
            Vector2 localPosition = Transform.TransformPointToLocal(position);
            
            float xPos = (float)Math.Floor((localPosition.X + 1) / FloorTileSize.X);
            float yPos = (float)Math.Floor((localPosition.Y + 1) / FloorTileSize.Y);

            return new Vector2(xPos, yPos);
        }

        public Transform GetBoxAtGridPosition(Vector2 boxGridPosition)
        {
            Transform localTransform;
            try
            {
                localTransform = FloorTiles[FloorRows - (int)boxGridPosition.Y - 1, (int)boxGridPosition.X].Transform;
            }
            catch(Exception)
            {
                throw new Exception("Found no FloorTile for row: " + boxGridPosition.Y + " and column: " + boxGridPosition.X);
            }
            
            Vector2 globalCenter = Transform.TransformPointToParent(localTransform.Position.X, localTransform.Position.Y);
            Vector2 globalSize = Transform.TransformSizeToParent(localTransform.Size.X, localTransform.Size.Y);

            return new Transform(globalCenter.X, globalCenter.Y, globalSize.X, globalSize.Y);
        }

        public Transform GetBoxAtPosition(Vector2 position)
        {
            float columnPosition = GetColumnOfPosition(position.X);
            float rowPosition = GetRowOfPosition(position.Y);

            return new Transform(columnPosition * FloorTileSize.X, rowPosition * FloorTileSize.Y, FloorTileSize.X, FloorTileSize.Y);
        }

        private float GetColumnOfPosition(float xPos)
        {
            Vector2 localPoint = Transform.TransformPointToLocal(xPos, 0);

            return (float)Math.Floor(localPoint.X / FloorTileSize.X);
        }

        private float GetRowOfPosition(float yPos)
        {
            Vector2 localPoint = Transform.TransformPointToLocal(0, yPos);

            return (float)Math.Floor(localPoint.X / FloorTileSize.X);
        }
    }
}
