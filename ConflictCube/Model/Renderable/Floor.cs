using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;

namespace ConflictCube.Model.Renderable
{
    public class Floor : RenderableLayer
    {
        //Static 
        public static RenderLayerType FloorLayer { get; private set; }

        static Floor()
        {
            FloorLayer = RenderLayerType.Floor;
        }

        public static Floor Instance(string pathToFloorData, Tileset<FloorTileType> tileset)
        {
            int levelRows, levelColumns;
            TileType[,] FloorTiles = GetFloorDataFromLevelfile(pathToFloorData, out levelRows, out levelColumns);
            return LoadFloor(levelRows, levelColumns, FloorTiles, tileset);
        }
        
        private static Floor LoadFloor(int levelRows, int levelColumns, TileType[,] FloorTiles, Tileset<FloorTileType> tileset)
        {
            TilesetTile currentTilesetTile;
            Floor floorOfLevel = new Floor(new Vector2(levelColumns, levelRows), tileset);

            for (int row = 0; row < levelRows; row++)
            {
                for (int column = 0; column < levelColumns; column++)
                {
                    floorOfLevel.Tileset.TilesetTiles.TryGetValue(FloorTiles[row, column], out currentTilesetTile);
                    
                    Box2D tileBox = floorOfLevel.BoxInFloorGrid(row, column);

                    FloorTile floorTile = new FloorTile(currentTilesetTile, tileBox, row, column);
                    floorOfLevel.AddFloorTile(floorTile, row, column);
                }
            }

            return floorOfLevel;
        }

        private Box2D BoxInFloorGrid(float row, float column)
        {
            float posX = -1 + column * FloorTileSize.X;
            float posY = -1 + ((FloorTiles.GetLength(0) - 1) - row) * FloorTileSize.Y;

            return new Box2D(posX, posY, FloorTileSize.X, FloorTileSize.Y);
        }

        private static TileType[,] GetFloorDataFromLevelfile(string levelPath, out int levelRows, out int levelColumns)
        {
            List<int[]> rows = new List<int[]>();

            using (StreamReader reader = new StreamReader(levelPath))
            {
                if (reader.EndOfStream)
                {
                    throw new Exception("Level file was empty.");
                }

                while (!reader.EndOfStream)
                {
                    rows.Add(CsvLineIntoIntArr(reader.ReadLine()));
                }
            }

            levelColumns = rows[0].Length;
            levelRows = rows.Count;

            TileType[,] ret = new TileType[levelRows, levelColumns];

            for (int i = 0; i < levelRows; i++)
            {
                for (int u = 0; u < levelColumns; u++)
                {
                    ret[i, u] = FloorTileType.GetTypeOfTileNumber(rows[i][u]);
                }
            }

            return ret;
        }

        private static int[] CsvLineIntoIntArr(string row)
        {
            string[] elements = row.Split(',');
            int[] iRow = new int[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                iRow[i] = int.Parse(elements[i]);
            }
            return iRow;
        }


        //Non - static
        public List<IMoveable> AttachedObjects { get; private set; }
        public Vector2 FloorTileSize;
        private Vector2 _FloorSize;
        public Vector2 FloorSize {
            get {
                return _FloorSize;
            }
            set {
                _FloorSize = value;
                FloorTileSize.X = 2 / _FloorSize.X;
                FloorTileSize.Y = 2 / _FloorSize.Y;

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
        public FloorTile[,] FloorTiles { get; set; }
        public Tileset<FloorTileType> Tileset { get; private set; }
        private float TotalMovedDistanceDown = 0;

        private Floor(Vector2 floorSize, Tileset<FloorTileType> floorTileset) : base(new List<RenderableObject>())
        {
            AttachedObjects = new List<IMoveable>();
            FloorTiles = new FloorTile[(int)floorSize.Y, (int)floorSize.X];
            FloorSize = floorSize;
            Tileset = floorTileset;
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
                attachedObject.Move(new Vector2(0.0f, distance * -1));
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
    }
}
