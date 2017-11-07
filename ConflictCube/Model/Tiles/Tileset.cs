using OpenTK;
using System;
using System.Collections.Generic;
using System.Xml;
using Zenseless.OpenGL;

namespace ConflictCube.Model.Tiles
{
    public static class TilesetLoader 
    {
        private static string TilesetTilecountAttrib = "tilecount";
        private static string TilesetTilecolumnsAttrib = "columns";
        
        public static Dictionary<TileType, Texture> LoadTileset(TilesetType type)
        {
            int tilesetCount, tilesetColumns;
            ReadTilesetDescription(TileTypeBase.GetTilesetDescriptionPath(type), out tilesetCount, out tilesetColumns);
            return ReadTileset(type, tilesetCount, tilesetColumns);
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

        private static Dictionary<TileType, Texture> ReadTileset(TilesetType type, int tilesetCount, int tilesetColumns)
        {
            string pathToPngTileset = TileTypeBase.GetTilesetPngPath(type);
            int tilesetRows = (int)(tilesetCount / tilesetColumns);

            int currentTilenumber = 0;
            Vector2 sizeOfTileInTileset = new Vector2();
            sizeOfTileInTileset.X = (float)1 / tilesetColumns;
            sizeOfTileInTileset.Y = (float)1 / tilesetRows;

            Dictionary<TileType, Texture> tilesetTiles = new Dictionary<TileType, Texture>();

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
                    Texture texture = (Texture)ZenselessWrapper.FromFile(pathToPngTileset, textureBox);

                    tilesetTiles.Add(TileTypeBase.GetTypeOfTileNumber(type, currentTilenumber), texture);
                }
            }

            return tilesetTiles;
        }
    }
}
