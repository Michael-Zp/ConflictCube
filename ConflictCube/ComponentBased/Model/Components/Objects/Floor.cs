using OpenTK;
using System;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;

namespace ConflictCube.ComponentBased
{
    public class Floor : GameObject
    {
        public readonly Vector2 FloorTileSize;
        public int FloorRows { get; set; }
        public int FloorColumns { get; set; }

        public LevelTile[,] FloorTiles { get; set; }
        public LevelTile[,] CubeTiles { get; set; }
        public float FloorBreakdownInterval { get; set; } = 3.0f;
        public CollisionGroup CollisionGroup;
        public Transform CurrentCheckpoint = new Transform(0, 0, 1, 1);

        private int FloorRowsBrokeDown = 0;
        private float LastFloorBreakdownTime = 0;
        private Player PlayerOnFloor = null;
        private int FloorRowThreshold = 3;
        public bool PlayerIsOverThreshold = false;
        public bool FloorShouldBreakDown = false;

        //floorSize is the size of the whole floor and not only the part which should be shown.
        public Floor(string name, Transform transform, GameObject parent, int rows, int columns, CollisionGroup group, Vector2 tileSize) : base(name, transform, parent)
        {
            FloorTiles = new LevelTile[rows, columns];
            CubeTiles = new LevelTile[rows, columns];
            FloorRows = rows;
            FloorColumns = columns;
            CollisionGroup = group;

            FloorTileSize = tileSize;
        }

        public override void OnUpdate()
        {
            if(PlayerOnFloor == null)
            {
                PlayerOnFloor = (Player)FindGameObjectByTypeInChildren<Player>();
            }

            if(PlayerOnFloor != null)
            {
                Vector2 playerPositionInGrid = GetGridPosition(PlayerOnFloor.Transform.TransformToGlobal());

                //If the level has to initialize itself dont start the floor breakdown.
                if(playerPositionInGrid.Y >= FloorRowThreshold && Time.Time.CurrentTime > 1)
                {
                    PlayerIsOverThreshold = true;
                }
            }
            
            if(FloorShouldBreakDown)
            {
                BreakDownFloor();
            }
        }

        public void BreakDownFloor()
        {
            if(!DebugGame.BreakDownFloors)
            {
                return;
            }

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


        public void AddLevelTile(LevelTile floorTile, LevelTile cubeTile, int y, int x)
        {
            FloorTiles[y, x] = floorTile;
            CubeTiles[y, x] = cubeTile;
            AddChild(floorTile);
            AddChild(cubeTile);
        }


        /// <summary>
        /// Return global start position
        /// </summary>
        /// <returns></returns>
        public Transform FindStartPosition()
        {
            int LowestRow = FloorTiles.GetLength(0);
            int LowestRowIndex = LowestRow - 1;

            for (int i = LowestRowIndex; i >= 0; i--)
            {
                for (int u = 0; u < FloorTiles.GetLength(1); u++)
                {
                    LevelTile tile = FloorTiles[i, u];
                    if (tile.Type == GameObjectType.Floor)
                    {
                        Vector2 startPos = tile.Transform.GetPosition(WorldRelation.Global);
                        return new Transform(startPos.X, startPos.Y, 1, 1);
                    }
                }
            }

            // No tile in the whole level is of type floor....
            throw new Exception("No start position found");
        }

        public Transform BoxInFloorGrid(float row, float column)
        {
            float posX = -1 + column * (FloorTileSize.X * 2.0f) + FloorTileSize.X;
            float posY = ((FloorTiles.GetLength(0) - 1) - row) * (FloorTileSize.Y * 2.0f) + FloorTileSize.Y;

            return new Transform(posX, posY, FloorTileSize.X, FloorTileSize.Y);
        }

        
        public Vector2 GetGridPosition(Transform globalPosition)
        {
            Transform localPosition = Transform.TransformToLocal(globalPosition);
           
            float columnPosition = GetColumnOfPosition(localPosition.GetPosition(WorldRelation.Local).X);
            float rowPosition = GetRowOfPosition(localPosition.GetPosition(WorldRelation.Local).Y);
            
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
                return FloorTiles[0, 0].Transform;
                throw new Exception("Found no FloorTile for row: " + boxGridPosition.Y + " and column: " + boxGridPosition.X);
            }
            
            return localTransform.TransformToGlobal();
        }

        public Transform GetBoxAtPosition(Transform globalPosition)
        {
            float columnPosition = GetColumnOfPosition(globalPosition.GetPosition(WorldRelation.Local).X);
            float rowPosition = GetRowOfPosition(globalPosition.GetPosition(WorldRelation.Local).Y);

            return (Transform)FloorTiles[(int)columnPosition, (int)rowPosition].Transform.Clone();
        }

        private float GetColumnOfPosition(float xPos)
        {
            float minX = FloorTiles[0, 0].Transform.GetMinX(WorldRelation.Local);
            float maxX = FloorTiles[0, FloorColumns - 1].Transform.GetMaxX(WorldRelation.Local);
            return (float)Math.Floor(((xPos - minX) / (maxX - minX)) * FloorColumns);
        }

        private float GetRowOfPosition(float yPos)
        {
            float minY = FloorTiles[FloorRows - 1, 0].Transform.GetMinY(WorldRelation.Local);
            float maxY = FloorTiles[0, 0].Transform.GetMaxY(WorldRelation.Local);
            return FloorRows - 1 - (float)Math.Floor(((yPos - minY) / (maxY - minY)) * FloorRows);
        }
    }
}
