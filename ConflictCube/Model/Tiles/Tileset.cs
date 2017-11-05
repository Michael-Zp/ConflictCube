using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zenseless.OpenGL;

namespace ConflictCube.Model.Tiles
{
    public class Tileset<T> where T : new()
    {
        private static string TilesetTilecountAttrib = "tilecount";
        private static string TilesetTilecolumnsAttrib = "columns";

        public Dictionary<TileType, TilesetTile> TilesetTiles = new Dictionary<TileType, TilesetTile>();
        public int TilesetCount { get; private set; }
        public int TilesetColumns { get; private set; }
        public int TilesetRows { get; private set; }

        public Tileset(string pathToDescription, string pathToPngTileset)
        {
            TilesetCount = -1;
            TilesetColumns = -1;
            ReadTilesetDescription(pathToDescription, pathToPngTileset);
            AddTilesToTileset(pathToPngTileset);
        }

        private void ReadTilesetDescription(string pathToDescription, string pathToPngTileset)
        {
            using (XmlTextReader reader = new XmlTextReader(pathToDescription))
            {
                while (reader.Read() && !(TilesetCount != -1 && TilesetColumns != -1))
                {
                    if (reader.HasAttributes)
                    {
                        do
                        {
                            if (reader.Name == TilesetTilecountAttrib)
                            {
                                TilesetCount = int.Parse(reader.Value);
                                continue;
                            }

                            if (reader.Name == TilesetTilecolumnsAttrib)
                            {
                                TilesetColumns = int.Parse(reader.Value);
                                continue;
                            }

                        } while (reader.MoveToNextAttribute() && !(TilesetCount != -1 && TilesetColumns != -1));
                    }
                }
            }

            if (TilesetCount == -1 || TilesetColumns == -1)
            {
                throw new Exception();
            }
        }

        private void AddTilesToTileset(string pathToPngTileset)
        {
            int tilesetRows = (int)(TilesetCount / TilesetColumns);

            int currentTilenumber = 0;
            Vector2 sizeOfTileInTileset = new Vector2();
            sizeOfTileInTileset.X = (float)1 / TilesetColumns;
            sizeOfTileInTileset.Y = (float)1 / tilesetRows;

            // Tiles are numbered from top left to bottom right in rows
            // Tileset -> 0    1
            //            2    3
            //            ...
            // Start at top (y = rows - 1) and at left x = 0
            for (int row = tilesetRows - 1; row >= 0; row--)
            {
                float uvYPos = (float)row * sizeOfTileInTileset.Y;

                for (int column = 0; column < TilesetColumns; column++, currentTilenumber++)
                {
                    float uvXPos = (float)column * sizeOfTileInTileset.X;
                    Box2d textureBox = new Box2d(uvXPos, uvYPos + sizeOfTileInTileset.Y, uvXPos + sizeOfTileInTileset.X, uvYPos);
                    Texture texture = (Texture)ZenselessWrapper.FromFile(pathToPngTileset, textureBox);

                    TileType type = TileTypeBase.GetTypeOfTileNumber<T>(currentTilenumber);

                    TilesetTiles.Add(type, new TilesetTile(type, texture));
                }
            }
        }
    }
}
