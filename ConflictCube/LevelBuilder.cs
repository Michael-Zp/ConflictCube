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

        public class Tile
        {
            public enum TileType
            {
                Floor,
                Wall,
                Hole,
                Finish
            }
            public static Vector2 Size { get; set; }

            public Texture Texture { get; private set; }
            public TileType Type { get; private set; }

            public Tile(TileType type, Texture texture)
            {
                Type = type;
                Texture = texture;
            }
        }

        public static Dictionary<int, Tile> Tileset = new Dictionary<int, Tile>();
        
        static LevelBuilder()
        {
            LoadTileset();
        }

        private static void LoadTileset()
        {
            int tilesetCount = -1, tilesetColumns = -1;
            using (XmlTextReader reader = new XmlTextReader(TilesetXmlPath))
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

            int tilesetRows = (int) (tilesetCount / tilesetColumns);

            Tile.Size = new Vector2(1 / (float)tilesetColumns, 1 / (float)tilesetRows);

            int currentTilenumber = 0;

            // Tiles are numbered from top left to bottom right in rows
            // Tileset -> 0    1
            //            2    3
            // Start at top (y = rows - 1) and at left x = 0
            for (int y = tilesetRows - 1; y >= 0; y--)
            {
                float uvYPos = y * Tile.Size.Y;

                for (int x = 0; x < tilesetColumns; x++, currentTilenumber++)
                {
                    float uvXPos = x * Tile.Size.X;
                    Box2d textureBox = new Box2d(uvXPos, uvYPos + Tile.Size.Y, uvXPos + Tile.Size.X, uvYPos);
                    Texture texture = (Texture)ZenselessWrapper.FromFile(TilesetPngPath, textureBox);
                    Tile.TileType type;
                    switch (currentTilenumber)
                    {
                        case 0:
                            type = Tile.TileType.Finish;
                            break;

                        case 1:
                            type = Tile.TileType.Floor;
                            break;

                        case 2:
                            type = Tile.TileType.Hole;
                            break;

                        case 3:
                            type = Tile.TileType.Wall;
                            break;

                        default:
                            throw new Exception("Tile type not known of Tile number: " + currentTilenumber);
                    }

                    Tile newTile = new Tile(type, texture);
                    Tileset.Add(currentTilenumber, newTile);
                }
            }
        }

        public static Level LoadLevel(int levelNumber)
        {
            string levelPath = LevelDirectoryPath + "Level" + levelNumber + ".csv";
            int[,] LevelTiles = GetDataFromLevelfile(levelPath);


            return new Level(LevelTiles, Tileset);
        }

        private static int[,] GetDataFromLevelfile(string levelPath)
        {
            List<int[]> rows = new List<int[]>();

            using (StreamReader reader = new StreamReader(levelPath))
            {
                if (reader.EndOfStream)
                {
                    return null;
                }
                
                while (!reader.EndOfStream)
                {
                    rows.Add(ConvertLevelRow(reader.ReadLine()));
                }
            }

            int colCount = rows[0].Length;
            int rowCount = rows.Count;

            int[,] ret = new int[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int u = 0; u < colCount; u++)
                {
                    ret[i, u] = rows[i][u];
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