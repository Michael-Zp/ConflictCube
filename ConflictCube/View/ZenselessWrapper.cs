using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Zenseless.HLGL;
using Zenseless.OpenGL;
using System.Drawing;
using System;
using System.IO;
using Zenseless.Geometry;

namespace ConflictCube
{
    using SysDraw = System.Drawing.Imaging;

    public static class ZenselessWrapper
    {

        public static ITexture TextureFromBitmapWithSpecificMapping(string bitmapPath, Box2d regionOfTexture)
        {
            var texture = new Texture2dGL();
            texture.Filter = TextureFilterMode.Mipmap;
            texture.Activate();
            using (Bitmap bmp = new Bitmap(bitmapPath))
            {
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                int left = (int)(bmp.Width * regionOfTexture.Left);
                int bottom = (int)(bmp.Height * regionOfTexture.Bottom);
                int sizeX = (int)(bmp.Width * regionOfTexture.Width);
                int sizeY = (int)(bmp.Height * regionOfTexture.Height);
                //var bmpData = bmp.LockBits(new Rectangle(left, bottom, sizeX, sizeY), ImageLockMode.ReadOnly, bmp.PixelFormat);
                var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), SysDraw.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var internalFormat = TextureLoader.SelectInternalPixelFormat(bmp.PixelFormat);
                var inputPixelFormat = TextureLoader.SelectPixelFormat(bmp.PixelFormat);

                //Mixes OpenGL 3 and 4. Might not be a good idea.
                texture.LoadPixels(bmpData.Scan0, bmpData.Width, bmpData.Height, internalFormat, inputPixelFormat, PixelType.UnsignedByte);

                bmp.UnlockBits(bmpData);

            }
            texture.Deactivate();
            return texture;
        }
        
        public static ITexture FromFile(string fileName, Box2d regionOfTexture)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException(fileName);
            }
            if (!File.Exists(fileName))
            {
                throw new FileLoadException(fileName);
            }
            return FromBitmap(new Bitmap(fileName), regionOfTexture);
        }

        public static ITexture FromBitmap(Bitmap bitmap, Box2d regionOfTexture)
        {
            ITexture texture;
            using (Bitmap bmp = new Bitmap(bitmap))
            {
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                int left = (int)(bmp.Width * regionOfTexture.Left);
                int bottom = (int)(bmp.Height * regionOfTexture.Bottom);
                int sizeX = (int)(bmp.Width * regionOfTexture.Width);
                int sizeY = (int)(bmp.Height * regionOfTexture.Height);
                using (Bitmap cropped = (Bitmap)bmp.Clone(new Rectangle(left, bottom, sizeX, sizeY), bmp.PixelFormat))
                {
                    texture = TextureLoader.FromBitmap(cropped);
                }
            }
            return texture;
        }

        public static bool Intersects(this Box2D box, float x, float y)
        {
            if (x <= box.MinX || box.MaxX <= x) return false;
            if (y <= box.MinY || box.MaxY <= y) return false;
            return true;
        }
    }
}
