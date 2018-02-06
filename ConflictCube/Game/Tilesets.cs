using Zenseless.HLGL;
using Zenseless.OpenGL;
using ConflictCube.ResxFiles;

namespace ConflictCube
{
    public class Tilesets
    {
        public SpriteSheet FiremanSheet;
        public SpriteSheet WelderSheet;
        public SpriteSheet FloorSheet;

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
            FiremanSheet = new SpriteSheet(TextureLoader.FromBitmap(TexturResource.FiremanTexture), 1, 1);
            WelderSheet = new SpriteSheet(TextureLoader.FromBitmap(TexturResource.WelderTexture), 1, 1);
            FloorSheet = new SpriteSheet(TextureLoader.FromBitmap(TexturResource.SpritesheetNewTextures), 5, 10, .98f, .98f);
        }
    }
}
