using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Floor Instance(string pathToFloorData, Dictionary<TileType, TilesetTile> tileset)
        {
            int levelRows, levelColumns;
            TileType[,] FloorTiles = GetFloorDataFromLevelfile(pathToFloorData, out levelRows, out levelColumns);
            return LoadFloor(levelRows, levelColumns, FloorTiles, tileset);
        }
        
        private static Floor LoadFloor(int levelRows, int levelColumns, TileType[,] FloorTiles, Dictionary<TileType, TilesetTile> tileset)
        {
            TilesetTile currentTile;
            Floor floorOfLevel = new Floor(new Vector2(levelColumns, levelRows), tileset);

            for (int y = 0; y < levelRows; y++)
            {
                float posY = 1 - (y + 1) * floorOfLevel.FloorTileSize.Y;
                for (int x = 0; x < levelColumns; x++)
                {
                    floorOfLevel.Tileset.TryGetValue(FloorTiles[y, x], out currentTile);

                    float posX = -1 + x * floorOfLevel.FloorTileSize.X;

                    FloorTile floorTile = new FloorTile(currentTile, floorOfLevel.FloorTileSize, new Vector2(posX, posY));
                    floorOfLevel.AddFloorTile(floorTile, y, x);
                }
            }

            return floorOfLevel;
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
            }
        }
        public FloorTile[,] FloorTiles { get; set; }
        public Dictionary<TileType, TilesetTile> Tileset { get; private set; }

        public Floor(Vector2 floorSize, Dictionary<TileType, TilesetTile> floorTileset) : base(new List<RenderableObject>())
        {
            FloorTiles = new FloorTile[(int)floorSize.Y, (int)floorSize.X];
            FloorSize = floorSize;
            Tileset = floorTileset;
        }

        public void MoveFloorUp(float distance)
        {
            foreach (FloorTile floorTile in FloorTiles)
            {
                floorTile.Box.MinY -= distance;
            }
        }
        

        public void AddFloorTile(FloorTile floorTile, int y, int x)
        {
            FloorTiles[y, x] = floorTile;
            ObjectsToRender.Add(floorTile);
        }
    }
}
