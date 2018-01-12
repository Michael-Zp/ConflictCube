using OpenTK.Graphics.OpenGL;
using OpenGL = OpenTK.Graphics.OpenGL;
using Zenseless.Geometry;
using System.Drawing;
using Zenseless.HLGL;

namespace ConflictCube.ComponentBased
{
    class OpenTKWrapper
    {
        public Color StandardColor = Color.White;

        private static OpenTKWrapper OpenTKWrapperInstance = null;

        public static OpenTKWrapper Instance()
        {
            if (OpenTKWrapperInstance == null)
            {
                OpenTKWrapperInstance = new OpenTKWrapper();
            }
            return OpenTKWrapperInstance;
        }

        public void DrawBoxWithAlphaChannel(Components.Rectangle rect, Color color)
        {
            EnableAlphaChannel();

            GL.Color4(color);

            DrawBox(rect);

            GL.Color4(StandardColor);

            DisableAlphaChannel();
        }

        public void DrawBox(Components.Rectangle rect, Color color, ITexture texture, Box2D uVCoordinates, bool alphaChannel = true)
        {
            if (alphaChannel)
            {
                EnableAlphaChannel();
            }
            
            GL.Color4(color);
            
            if (texture != null)
            {
                EnableTextures();
                texture.Activate();

                DrawTexturedBox(rect, uVCoordinates);

                texture.Deactivate();
                DisableTextures();
            }
            else
            {
                DrawBox(rect);
            }
            

            GL.Color4(StandardColor);

            if (alphaChannel)
            {
                DisableAlphaChannel();
            }
        }

        private void DrawTexturedBox(Components.Rectangle rect, Box2D uVCoordinates)
        {
            GL.Begin(PrimitiveType.Quads);

            //Bottom left
            GL.TexCoord2(uVCoordinates.MinX, uVCoordinates.MinY);
            GL.Vertex2(rect.BottomLeft);

            //Bottom right
            GL.TexCoord2(uVCoordinates.MaxX, uVCoordinates.MinY);
            GL.Vertex2(rect.BottomRight);

            //Top right
            GL.TexCoord2(uVCoordinates.MaxX, uVCoordinates.MaxY);
            GL.Vertex2(rect.TopRight);

            //Top left
            GL.TexCoord2(uVCoordinates.MinX, uVCoordinates.MaxY);
            GL.Vertex2(rect.TopLeft);

            GL.End();
        }

        private void DrawBox(Components.Rectangle rect)
        {
            GL.Begin(PrimitiveType.Quads);

            //Bottom left
            GL.Vertex2(rect.BottomLeft);

            //Bottom right
            GL.Vertex2(rect.BottomRight);

            //Top right
            GL.Vertex2(rect.TopRight);

            //Top left
            GL.Vertex2(rect.TopLeft);

            GL.End();
        }

        public void PrintText(float xPos, float yPos, float xSize, float ySize, string text)
        {
            EnableAlphaChannel();
            EnableTextures();
            
            Font.Instance().TextureFont.PrintWithSize(xPos, yPos, 0f, xSize, ySize, 1f, text);

            DisableTextures();
            DisableAlphaChannel();
        }


        private void EnableAlphaChannel()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, OpenGL.BlendingFactorDest.OneMinusSrcAlpha);
        }

        private void DisableAlphaChannel()
        {
            GL.Disable(OpenGL.EnableCap.Blend);
        }

        private void EnableTextures()
        {
            GL.Enable(OpenGL.EnableCap.Texture2D);
        }

        private void DisableTextures()
        {
            GL.Disable(OpenGL.EnableCap.Texture2D);
        }
    }
}
