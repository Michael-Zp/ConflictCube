using OpenTK.Graphics.OpenGL;
using OpenGL = OpenTK.Graphics.OpenGL;
using Zenseless.Geometry;
using System.Drawing;
using ConflictCube.ComponentBased.Components;
using Zenseless.HLGL;
using ConflictCube.ComponentBased.Model.Components.UI;
using OpenTK;

namespace ConflictCube.ComponentBased
{
    class OpenTKWrapper
    {
        public Color StandardColor = Color.White;

        private static OpenTKWrapper OpenTKWrapperInstance = null;

        public static OpenTKWrapper Instance()
        {
            if(OpenTKWrapperInstance == null)
            {
                OpenTKWrapperInstance = new OpenTKWrapper();
            }
            return OpenTKWrapperInstance;
        }

        public void DrawBoxWithAlphaChannel(Transform transform, Color color)
        {
            EnableAlphaChannel();
            
            GL.Color4(color);
            
            GL.Begin(OpenGL.PrimitiveType.Quads);

            //Bottom left
            GL.Vertex2(transform.MinX, transform.MinY);

            //Bottom right
            GL.Vertex2(transform.MaxX, transform.MinY);

            //Top right
            GL.Vertex2(transform.MaxX, transform.MaxY);

            //Top left
            GL.Vertex2(transform.MinX, transform.MaxY);

            GL.End();

            GL.Color4(StandardColor);

            DisableAlphaChannel();
        }

        public void DrawBoxWithTextureAndAlphaChannel(Transform transform, ITexture texture, Box2D uVCoordinates, Color color)
        {
            EnableAlphaChannel();

            EnableTextures();

            texture.Activate();
            
            GL.Color4(color);

            GL.Begin(OpenGL.PrimitiveType.Quads);

            //Bottom left
            GL.TexCoord2(uVCoordinates.MinX, uVCoordinates.MinY);
            GL.Vertex2(transform.MinX, transform.MinY);

            //Bottom right
            GL.TexCoord2(uVCoordinates.MaxX, uVCoordinates.MinY);
            GL.Vertex2(transform.MaxX, transform.MinY);

            //Top right
            GL.TexCoord2(uVCoordinates.MaxX, uVCoordinates.MaxY);
            GL.Vertex2(transform.MaxX, transform.MaxY);

            //Top left
            GL.TexCoord2(uVCoordinates.MinX, uVCoordinates.MaxY);
            GL.Vertex2(transform.MinX, transform.MaxY);

            GL.End();
            
            texture.Deactivate();

            GL.Color4(StandardColor);

            DisableTextures();

            DisableAlphaChannel();
        }

        public void PrintText(float xPos, float yPos, float xSize, float ySize, string text)
        {
            EnableAlphaChannel();
            EnableTextures();

            //Font.Instance().TextureFont.Print(xPos, yPos, 0f, xSize, text);
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
