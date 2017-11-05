using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;
using Zenseless.OpenGL;

namespace ConflictCube
{
    public class Player : RenderableObject
    {
        private static string PlayerTilesetDescriptionPath = LevelBuilder.LevelDirectoryPath + "Player.tsx";
        private static string PlayerTilesetPngPath = LevelBuilder.LevelDirectoryPath + "Player.gif";

        public static Tileset<PlayerTileType> PlayerTileset { get; set; }
        public static TileType DefaultTileType = TileType.Player;

        static Player()
        {
            PlayerTileset = new Tileset<PlayerTileType>(PlayerTilesetDescriptionPath, PlayerTilesetPngPath);
        }

        public Player(TilesetTile tile, Vector2 size, Vector2 position) : base(position, size, tile.Texture)
        {
        }
    }
}