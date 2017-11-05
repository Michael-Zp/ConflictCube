using GL3 = OpenTK.Graphics.OpenGL.GL;
using OpenGL3 = OpenTK.Graphics.OpenGL;
using Zenseless.Geometry;
using System.Drawing;
using Zenseless.OpenGL;

namespace ConflictCube
{
    class OpenTKWrapper
    {
        public static void DrawBox(Box2D rect, Color color)
        {
            GL3.Color3(color);
            GL3.Begin(OpenGL3.PrimitiveType.Quads);
            GL3.Vertex2(rect.MinX, rect.MinY);
            GL3.Vertex2(rect.MaxX, rect.MinY);
            GL3.Vertex2(rect.MaxX, rect.MaxY);
            GL3.Vertex2(rect.MinX, rect.MaxY);
            GL3.End();
        }
        

        public static void DrawBoxWithTexture(Box2D rect, Texture texture)
        {
            GL3.Enable(OpenGL3.EnableCap.Texture2D);
            texture.Activate();
            GL3.Begin(OpenGL3.PrimitiveType.Quads);
            //Bottom left
            GL3.TexCoord2(0, 1);
            GL3.Vertex2(rect.MinX, rect.MinY);
            //Bottom right
            GL3.TexCoord2(1, 1);
            GL3.Vertex2(rect.MaxX, rect.MinY);
            //Top right
            GL3.TexCoord2(1, 0);
            GL3.Vertex2(rect.MaxX, rect.MaxY);
            //Top left
            GL3.TexCoord2(0, 0);
            GL3.Vertex2(rect.MinX, rect.MaxY);

            GL3.End();
            texture.Deactivate();
            GL3.Disable(OpenGL3.EnableCap.Texture2D);
        }

        public static void DrawBoxWithTextureAndAlphaChannel(Box2D rect, Texture texture)
        {
            GL3.Enable(OpenGL3.EnableCap.Blend);
            GL3.BlendFunc(OpenGL3.BlendingFactorSrc.SrcAlpha, OpenGL3.BlendingFactorDest.OneMinusSrcAlpha);
            DrawBoxWithTexture(rect, texture);
            GL3.Disable(OpenGL3.EnableCap.Blend);
        }
    }
}
