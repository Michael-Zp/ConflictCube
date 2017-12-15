using Zenseless.OpenGL;
using static ConflictCube.ComponentBased.View.ZenselessWrapper;

namespace ConflictCube.ComponentBased
{
    public class Font
    {
        public MyTextureFont TextureFont;

        private static Font FontInstance = null;


        public static Font Instance()
        {
            if(FontInstance == null)
            {
                FontInstance = new Font();
            }
            return FontInstance;
        }

        public Font()
        {
            TextureFont = new MyTextureFont(TextureLoader.FromBitmap(FontResources.Font1), 10, 32, .8f, 1, .7f);
        }
    }
}
