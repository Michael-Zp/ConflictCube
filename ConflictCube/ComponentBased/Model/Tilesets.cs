using Zenseless.HLGL;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased
{
    public class Tilesets
    {
        public SpriteSheet FloorSheet;
        public SpriteSheet FloorSheetIceFire;
        public SpriteSheet PlayerSheet;
        public SpriteSheet InventoryTextures;

        private static Tilesets TilesetsInstance = null;

        public static Tilesets Instance()
        {
            if(TilesetsInstance == null)
            {
                TilesetsInstance = new Tilesets();
            }
            return TilesetsInstance;
        }

        private Tilesets()
        {
            FloorSheet = new SpriteSheet(TextureLoader.FromBitmap(TexturResource.FloorTileset), 2, 2);
            FloorSheetIceFire = new SpriteSheet(TextureLoader.FromBitmap(TexturResource.FloorTilesetIceFire), 2, 4);
            PlayerSheet = new SpriteSheet(TextureLoader.FromBitmap(TexturResource.PlayerTexture), 1, 1);
            InventoryTextures = new SpriteSheet(TextureLoader.FromBitmap(InventoryResources.Sledgehammer), 1, 1);
        }
    }
}
