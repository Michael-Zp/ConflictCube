﻿using OpenTK;
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
        public CollisionGroup CollisionGroup;


        private int FloorRowsBrokeDown = 0;
        private float LastFloorBreakdownTime = 0;

        //floorSize is the size of the whole floor and not only the part which should be shown.
        public Floor(string name, Transform transform, GameObject parent, int rows, int columns, CollisionGroup group) : base(name, transform, parent)
        {
            FloorTiles = new FloorTile[rows, columns];
            FloorRows = rows;
            FloorColumns = columns;
            CollisionGroup = group;
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

        public Vector2 GetGridPosition(Transform globalPosition)
        {
            Transform localPosition = Transform.TransformToLocal(globalPosition);

            float columnPosition = GetColumnOfPosition(localPosition.Position.X);
            float rowPosition = GetRowOfPosition(localPosition.Position.Y);

            return new Vector2(columnPosition, rowPosition);
        }


        /// <summary>
        /// Returns the global transform of a box on the floor at this grid position.
        /// </summary>
        /// <param name="boxGridPosition"></param>
        /// <returns></returns>
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
            
            return localTransform.TransformToGlobal();
        }

        public Transform GetBoxAtPosition(Transform globalPosition)
        {
            globalPosition = Transform.TransformToLocal(globalPosition);

            float columnPosition = GetColumnOfPosition(globalPosition.Position.X);
            float rowPosition = GetRowOfPosition(globalPosition.Position.Y);

            return (Transform)FloorTiles[(int)columnPosition, (int)rowPosition].Transform.Clone();
        }

        private float GetColumnOfPosition(float xPos)
        {
            return (float)Math.Floor((xPos + 1) / (FloorTileSize.X * 2));
        }

        private float GetRowOfPosition(float yPos)
        {
            return (float)Math.Floor((yPos + 1) / (FloorTileSize.Y * 2));
        }
    }
}
