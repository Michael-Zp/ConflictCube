using OpenTK;
using ConflictCube.Model.Renderable;
using Zenseless.OpenGL;

namespace ConflictCube.Model.Tiles
{
    public enum TileType
    {
        Floor,
        Wall,
        Hole,
        Finish
    }

    public class FloorTileType
    {
        private static TileType[] FloorNumberToType = { TileType.Finish, TileType.Floor, TileType.Hole, TileType.Wall };

        public static TileType GetTypeOfTileNumber(int tileNumber)
        {
            return FloorNumberToType[tileNumber];
        }
    }

    public class Tile
    {
        public TileType Type { get; private set; }
        public Texture Texture { get; private set; }


        public Tile(TileType type, Texture texture)
        {
            Type = type;
            Texture = texture;
        }

    }


    public class FloorTile : RenderableObject
    {
        public TileType Type { get; private set; }

        public FloorTile(Tile tile, Vector2 size, Vector2 position) : base(position, size, tile.Texture)
        {
            if (tile.Type != TileType.Finish &&
                tile.Type != TileType.Floor &&
                tile.Type != TileType.Hole &&
                tile.Type != TileType.Wall)
            {
                throw new System.Exception("FloorTile was initalized with wrong TileType");
            }

            Type = tile.Type;
        }
    }
}
