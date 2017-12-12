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
                
                FloorTileSize.Y = 1.0f / _FloorRows;
            }
        }
        private int _FloorColumns;
        public int FloorColumns {
            get {
                return _FloorColumns;
            }
            set {
                _FloorColumns = value;

                FloorTileSize.X = 1.0f / _FloorColumns;
            }
        }

        public FloorTile[,] FloorTiles { get; set; }
        public float FloorBreakdownInterval { get; set; } = 3.0f;


        private int FloorRowsBrokeDown = 0;
        private float LastFloorBreakdownTime = 0;

        //floorSize is the size of the whole floor and not only the part which should be shown.
        public Floor(string name, Transform transform, GameObject parent, int rows, int columns) : base(name, transform, parent)
        {
            FloorTiles = new FloorTile[rows, columns];
            FloorRows = rows;
            FloorColumns = columns;
        }

        public override void OnUpdate()
        {
            //MoveFloorUp();
        }

        public void MoveFloorUp()
        {
            if(Time.Time.CurrentTime - LastFloorBreakdownTime > FloorBreakdownInterval)
            {
                LastFloorBreakdownTime = Time.Time.CurrentTime;
                //Break down floor

                int lastFloorRowIndex = FloorTiles.GetLength(0) - 1;
                int indexRowToBreakDown = lastFloorRowIndex - FloorRowsBrokeDown;

                for (int i = 0; i < FloorColumns; i++)
                {
                    FloorTiles[indexRowToBreakDown, i].ChangeFloorTile(GameObjectType.Hole);                    
                }

                FloorRowsBrokeDown++;
            }
        }


        public void AddFloorTile(FloorTile floorTile, int y, int x)
        {
            FloorTiles[y, x] = floorTile;
            AddChild(floorTile);
        }

        public Vector2 FindStartPosition()
        {
            int LowestRow = FloorTiles.GetLength(0);
            int LowestRowIndex = LowestRow - 1;

            for (int i = LowestRowIndex; i >= 0; i--)
            {
                for (int u = 0; u < FloorTiles.GetLength(1); u++)
                {
                    FloorTile tile = FloorTiles[i, u];
                    if (tile.Type == GameObjectType.Floor)
                    {
                        Transform startPos = new Transform(tile.Transform.Position.X, tile.Transform.Position.Y, 0, 0);

                        return Transform.TransformToGlobal(startPos).Position;
                    }
                }
            }

            // No tile in the whole level is of type floor....
            throw new Exception("No start position found");
        }

        public Transform BoxInFloorGrid(float row, float column)
        {
            float posX = -1 + column * (FloorTileSize.X * 2.0f) + FloorTileSize.X;
            float posY = -1 + ((FloorTiles.GetLength(0) - 1) - row) * (FloorTileSize.Y * 2.0f) + FloorTileSize.Y;

            return new Transform(posX, posY, FloorTileSize.X, FloorTileSize.Y);
        }

        public Vector2 GetGridPosition(Vector2 position)
        {
            Vector2 localPosition = Transform.TransformToLocal(new Transform(position.X, position.Y, 0, 0)).Position;
            
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
            
            return localTransform.TransformToParent();
        }

        public Transform GetBoxAtPosition(Vector2 position)
        {
            float columnPosition = GetColumnOfPosition(position.X);
            float rowPosition = GetRowOfPosition(position.Y);

            return new Transform(columnPosition * FloorTileSize.X, rowPosition * FloorTileSize.Y, FloorTileSize.X, FloorTileSize.Y);
        }

        private float GetColumnOfPosition(float xPos)
        {
            Vector2 localPoint = Transform.TransformToLocal(new Transform(xPos, 0, 0, 0)).Position;

            return (float)Math.Floor(localPoint.X / FloorTileSize.X);
        }

        private float GetRowOfPosition(float yPos)
        {
            Vector2 localPoint = Transform.TransformToLocal(new Transform(0, yPos, 0, 0)).Position;

            return (float)Math.Floor(localPoint.X / FloorTileSize.X);
        }
    }
}
