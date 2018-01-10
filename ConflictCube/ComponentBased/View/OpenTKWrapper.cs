using OpenTK.Graphics.OpenGL;
using OpenGL = OpenTK.Graphics.OpenGL;
using Zenseless.Geometry;
using System.Drawing;
using ConflictCube.ComponentBased.Components;
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

        public void DrawBoxWithAlphaChannel(Transform transform, Color color)
        {
            EnableAlphaChannel();

            GL.Color4(color);

            DrawBox(transform);

            GL.Color4(StandardColor);

            DisableAlphaChannel();
        }

        public void DrawBox(Transform transform, Color color, ITexture texture, Box2D uVCoordinates, bool alphaChannel = true)
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

                DrawTexturedBox(transform, uVCoordinates);

                texture.Deactivate();
                DisableTextures();
            }
            else
            {
                DrawBox(transform);
            }
            

            GL.Color4(StandardColor);

            if (alphaChannel)
            {
                DisableAlphaChannel();
            }
        }

        private void DrawTexturedBox(Transform transform, Box2D uVCoordinates)
        {
            GL.Begin(PrimitiveType.Quads);

            //Bottom left
            GL.TexCoord2(uVCoordinates.MinX, uVCoordinates.MinY);
            GL.Vertex2(transform.GetMinX(WorldRelation.Global), transform.GetMinY(WorldRelation.Global));

            //Bottom right
            GL.TexCoord2(uVCoordinates.MaxX, uVCoordinates.MinY);
            GL.Vertex2(transform.GetMaxX(WorldRelation.Global), transform.GetMinY(WorldRelation.Global));

            //Top right
            GL.TexCoord2(uVCoordinates.MaxX, uVCoordinates.MaxY);
            GL.Vertex2(transform.GetMaxX(WorldRelation.Global), transform.GetMaxY(WorldRelation.Global));

            //Top left
            GL.TexCoord2(uVCoordinates.MinX, uVCoordinates.MaxY);
            GL.Vertex2(transform.GetMinX(WorldRelation.Global), transform.GetMaxY(WorldRelation.Global));

            GL.End();
        }

        private void DrawBox(Transform transform)
        {
            GL.Begin(PrimitiveType.Quads);

            //Bottom left
            GL.Vertex2(transform.GetMinX(WorldRelation.Global), transform.GetMinY(WorldRelation.Global));

            //Bottom right
            GL.Vertex2(transform.GetMaxX(WorldRelation.Global), transform.GetMinY(WorldRelation.Global));

            //Top right
            GL.Vertex2(transform.GetMaxX(WorldRelation.Global), transform.GetMaxY(WorldRelation.Global));

            //Top left
            GL.Vertex2(transform.GetMinX(WorldRelation.Global), transform.GetMaxY(WorldRelation.Global));

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
