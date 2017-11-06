using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;

namespace ConflictCube
{
    public class Player : RenderableObject, IMoveable
    {
        private static string PlayerTilesetDescriptionPath = LevelBuilder.LevelDirectoryPath + "Player.tsx";
        private static string PlayerTilesetPngPath = LevelBuilder.LevelDirectoryPath + "Player.gif";

        public static Tileset<PlayerTileType> PlayerTileset { get; set; }
        public float Speed { get; private set; }

        public static TileType DefaultTileType = TileType.Player;

        static Player()
        {
            PlayerTileset = new Tileset<PlayerTileType>(PlayerTilesetDescriptionPath, PlayerTilesetPngPath);
        }

        public Player(TilesetTile tile, Vector2 size, Vector2 position, float speed) : base(position, size, tile.Texture)
        {
            Speed = speed;
        }

        public void Move(Vector2 moveVector)
        {
            Box.MinX += moveVector.X;
            Box.MinY += moveVector.Y;
        }

        public void SetPosition(Vector2 position)
        {
            Box.CenterX = position.X;
            Box.CenterY = position.Y;
        }
    }
}