using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using Zenseless.Geometry;

namespace ConflictCube.Model.Renderable
{
    public static class FloorLoader
    {
        public static RenderLayerType FloorLayer { get; private set; }

        static FloorLoader()
        {
            FloorLayer = RenderLayerType.Floor;
        }

        public static Floor Instance(string pathToFloorData, Box2D floorBox)
        {
            int levelRows, levelColumns;
            TileType[,] FloorTiles = GetFloorDataFromLevelfile(pathToFloorData, out levelRows, out levelColumns);
            return LoadFloor(levelRows, levelColumns, FloorTiles, floorBox);
        }

        private static Floor LoadFloor(int levelRows, int levelColumns, TileType[,] floorTiles, Box2D areaOfFloor)
        {
            Floor floorOfLevel = new Floor(new Vector2(levelColumns, levelRows), areaOfFloor);

            for (int row = 0; row < levelRows; row++)
            {
                for (int column = 0; column < levelColumns; column++)
                {
                    Box2D tileBox = floorOfLevel.BoxInFloorGrid(row, column);

                    FloorTile floorTile = new FloorTile(floorTiles[row, column], tileBox, row, column);
                    floorOfLevel.AddFloorTile(floorTile, row, column);
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
    }
}
