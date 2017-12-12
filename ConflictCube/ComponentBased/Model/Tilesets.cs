using System;
using System.Collections.Generic;
using Zenseless.HLGL;
using Zenseless.OpenGL;
using Zenseless.Geometry;

namespace ConflictCube.ComponentBased
{
    public static class Tilesets
    {
        public static SpriteSheet FloorSheet;
        public static SpriteSheet PlayerSheet;

        static Tilesets()
        {
            FloorSheet  = new SpriteSheet(TextureLoader.FromBitmap(TexturResource.FloorTileset ), 2, 2);
            PlayerSheet = new SpriteSheet(TextureLoader.FromBitmap(TexturResource.PlayerTexture), 1, 1);
        }

        public static List<Tuple<Texture, Box2D>> GetAllTextures()
        {
            List<Tuple<Texture, Box2D>> textures = new List<Tuple<Texture, Box2D>>();

            textures.AddRange(GetTexturesOfSheet(FloorSheet));
            textures.AddRange(GetTexturesOfSheet(PlayerSheet));

            return textures;
        }

        private static List<Tuple<Texture, Box2D>> GetTexturesOfSheet(SpriteSheet sheet)
        {
            List<Tuple<Texture, Box2D>> textures = new List<Tuple<Texture, Box2D>>();

            for (uint col = 0; col < sheet.SpritesPerColumn; col++)
            {
                for (uint row = 0; row < sheet.SpritesPerRow; row++)
                {
                    textures.Add(Tuple.Create((Texture)sheet.Tex, sheet.CalcSpriteTexCoords(col * sheet.SpritesPerColumn + row)));
                }
            }

            return textures;
        }
    }
}
