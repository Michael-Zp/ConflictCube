using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Zenseless.OpenGL;

namespace ConflictCube
{
    public class LevelBuilder
    {
        private static string LevelDirectoryPath = ".\\ConflictCube\\Levels\\";
        private static string TilesetXmlPath = LevelDirectoryPath + "Tileset.tsx";
        private static string TilesetTilecountAttrib = "tilecount";
        private static string TilesetTilecolumnsAttrib = "columns";
        private static string TilesetPngPath = LevelDirectoryPath + "Tileset.png";

        

        public static Dictionary<TileType, Tile> FloorTileset = new Dictionary<TileType, Tile>();
        
        static LevelBuilder()
        {
            LoadFloorTileset();
        }

        private static void LoadFloorTileset()
        {
            int tilesetCount, tilesetColumns;

            ReadTilesetDescription(TilesetXmlPath, out tilesetCount, out tilesetColumns);

            int tilesetRows = (int) (tilesetCount / tilesetColumns);

            int currentTilenumber = 0;
            Vector2 sizeOfTileInTileset = new Vector2();
            sizeOfTileInTileset.X = (float)1 / tilesetColumns;
            sizeOfTileInTileset.Y = (float)1 / tilesetRows;

            // Tiles are numbered from top left to bottom right in rows
            // Tileset -> 0    1
            //            2    3
            // Start at top (y = rows - 1) and at left x = 0
            for (int y = tilesetRows - 1; y >= 0; y--)
            {
                float uvYPos = (float)y * sizeOfTileInTileset.Y;

                for (int x = 0; x < tilesetColumns; x++, currentTilenumber++)
                {
                    float uvXPos = (float)x * sizeOfTileInTileset.X;
                    Box2d textureBox = new Box2d(uvXPos, uvYPos + sizeOfTileInTileset.Y, uvXPos + sizeOfTileInTileset.X, uvYPos);
                    Texture texture = (Texture)ZenselessWrapper.FromFile(TilesetPngPath, textureBox);

                    TileType type = FloorTileType.GetTypeOfTileNumber(currentTilenumber);

                    Tile newTile = new Tile(type, texture);
                    FloorTileset.Add(type, newTile);
                }
            }
        }

        private static void ReadTilesetDescription(string pathToDescription, out int tilesetCount, out int tilesetColumns)
        {
            tilesetCount = -1;
            tilesetColumns = -1;
            using (XmlTextReader reader = new XmlTextReader(pathToDescription))
            {
                while (reader.Read())
                {
                    if (reader.HasAttributes)
                    {
                        do
                        {
                            if (reader.Name == TilesetTilecountAttrib)
                            {
                                tilesetCount = int.Parse(reader.Value);
                                continue;
                            }

                            if (reader.Name == TilesetTilecolumnsAttrib)
                            {
                                tilesetColumns = int.Parse(reader.Value);
                                continue;
                            }

                        } while (reader.MoveToNextAttribute() && !(tilesetCount != -1 && tilesetColumns != -1));
                    }
                }
            }

            if (tilesetCount == -1 || tilesetColumns == -1)
            {
                throw new Exception();
            }
        }

        public static Level LoadLevel(int levelNumber)
        {
            Level newLevel = new Level();
            string levelPath = LevelDirectoryPath + "Level" + levelNumber + ".csv";
            int levelRows, levelColumns;
            TileType[,] FloorTiles = GetFloorDataFromLevelfile(levelPath, out levelRows, out levelColumns);
          
            Tile currentTile;
            Floor floorOfLevel = new Floor(new Vector2(levelColumns, levelRows), FloorTileset);

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
            
            newLevel.Floor = floorOfLevel;

            return newLevel;
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
                    rows.Add(ConvertLevelRow(reader.ReadLine()));
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

        private static int[] ConvertLevelRow(string row)
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