using OpenTK;
using System;
using Engine.Components;

namespace ConflictCube.Objects
{
    public class Floor : GameObject
    {
        public readonly Vector2 FloorTileSize;
        public int FloorRows { get; set; }
        public int FloorColumns { get; set; }

        public LevelTile[,] FloorTiles { get; set; }
        public LevelTile[,] ButtonTiles { get; set; }
        public LevelTile[,] CubeTiles { get; set; }
        public CollisionGroup CollisionGroup;
        public Transform BluePlayerCheckpoint;
        public Transform OrangePlayerCheckpoint;

        //floorSize is the size of the whole floor and not only the part which should be shown.
        public Floor(string name, Transform transform, int rows, int columns, CollisionGroup group, Vector2 tileSize, GameObject parent) : base(name, transform, parent)
        {
            FloorTiles = new LevelTile[rows, columns];
            ButtonTiles = new LevelTile[rows, columns];
            CubeTiles = new LevelTile[rows, columns];
            FloorRows = rows;
            FloorColumns = columns;
            CollisionGroup = group;

            FloorTileSize = tileSize;
        }

        public void AddLevelTile(LevelTile floorTile, LevelTile buttonTile, LevelTile cubeTile, int y, int x)
        {
            FloorTiles[y, x] = floorTile;
            ButtonTiles[y, x] = buttonTile;
            CubeTiles[y, x] = cubeTile;
            floorTile.Parent = this;
            buttonTile.Parent = this;
            cubeTile.Parent = this;
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
                localTransform = FloorTiles[(int)boxGridPosition.Y, (int)boxGridPosition.X].Transform;
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
