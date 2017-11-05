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

        

        public static Dictionary<TileType, TilesetTile> FloorTileset = new Dictionary<TileType, TilesetTile>();
        
        static LevelBuilder()
        {
            LoadFloorTileset();
        }

        private static void LoadFloorTileset()
        {
            int tilesetCount, tilesetColumns;
            ReadTilesetDescription(TilesetXmlPath, out tilesetCount, out tilesetColumns);
            AddTilesToTileset<FloorTileType>(tilesetCount, tilesetColumns);
        }
        

        private static void ReadTilesetDescription(string pathToDescription, out int tilesetCount, out int tilesetColumns)
        {
            tilesetCount = -1;
            tilesetColumns = -1;
            using (XmlTextReader reader = new XmlTextReader(pathToDescription))
            {
                while (reader.Read() && !(tilesetCount != -1 && tilesetColumns != -1))
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

        private static void AddTilesToTileset<T>(int tilesetCount, int tilesetColumns) where T : new()
        {
            int tilesetRows = (int)(tilesetCount / tilesetColumns);

            int currentTilenumber = 0;
            Vector2 sizeOfTileInTileset = new Vector2();
            sizeOfTileInTileset.X = (float)1 / tilesetColumns;
            sizeOfTileInTileset.Y = (float)1 / tilesetRows;

            // Tiles are numbered from top left to bottom right in rows
            // Tileset -> 0    1
            //            2    3
            //            ...
            // Start at top (y = rows - 1) and at left x = 0
            for (int row = tilesetRows - 1; row >= 0; row--)
            {
                float uvYPos = (float)row * sizeOfTileInTileset.Y;

                for (int column = 0; column < tilesetColumns; column++, currentTilenumber++)
                {
                    float uvXPos = (float)column * sizeOfTileInTileset.X;
                    Box2d textureBox = new Box2d(uvXPos, uvYPos + sizeOfTileInTileset.Y, uvXPos + sizeOfTileInTileset.X, uvYPos);
                    Texture texture = (Texture)ZenselessWrapper.FromFile(TilesetPngPath, textureBox);

                    TileType type = TileTypeBase.GetTypeOfTileNumber<T>(currentTilenumber);

                    FloorTileset.Add(type, new TilesetTile(type, texture));
                }
            }
        }

        public static Level LoadLevel(int levelNumber)
        {
            Level newLevel = new Level();
            string levelPath = LevelDirectoryPath + "Level" + levelNumber + ".csv";

            newLevel.Floor = Floor.Instance(levelPath, FloorTileset);

            return newLevel;
        }

    }
}