using OpenTK.Graphics.OpenGL;
using OpenGL = OpenTK.Graphics.OpenGL;
using Zenseless.Geometry;
using System.Drawing;
using Zenseless.OpenGL;
using ConflictCube.ComponentBased.Components;
using System;
using Zenseless.HLGL;

namespace ConflictCube.ComponentBased
{
    class OpenTKWrapper
    {
        public static Color StandardColor = Color.White;

        public static void DrawBox(Box2D rect, Color color)
        {
            GL.Color3(color);
            DrawBoxVertices(rect);
            GL.Color3(Color.White);
        }

        private static void DrawBoxVertices(Box2D rect)
        {
            GL.Begin(OpenGL.PrimitiveType.Quads);
            GL.Vertex2(rect.MinX, rect.MinY);
            GL.Vertex2(rect.MaxX, rect.MinY);
            GL.Vertex2(rect.MaxX, rect.MaxY);
            GL.Vertex2(rect.MinX, rect.MaxY);
            GL.End();
        }
        

        public static void DrawBoxWithTexture(Box2D rect, Texture texture)
        {
            GL.Enable(OpenGL.EnableCap.Texture2D);
            texture.Activate();
            GL.Begin(OpenGL.PrimitiveType.Quads);
            //Bottom left
            GL.TexCoord2(0, 1);
            GL.Vertex2(rect.MinX, rect.MinY);
            //Bottom right
            GL.TexCoord2(1, 1);
            GL.Vertex2(rect.MaxX, rect.MinY);
            //Top right
            GL.TexCoord2(1, 0);
            GL.Vertex2(rect.MaxX, rect.MaxY);
            //Top left
            GL.TexCoord2(0, 0);
            GL.Vertex2(rect.MinX, rect.MaxY);

            GL.End();
            texture.Deactivate();
            GL.Disable(OpenGL.EnableCap.Texture2D);
        }

        public static void DrawBoxWithAlphaChannel(Transform transform, Color color)
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

        public static void DrawBoxWithTextureAndAlphaChannel(Transform transform, ITexture texture, Box2D uVCoordinates, Color color)
        {
            EnableAlphaChannel();

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

            DisableAlphaChannel();
        }

        public static void DrawBoxWithTextureAndAlphaChannel(Box2D rect, Texture texture)
        {
            EnableAlphaChannel();
            DrawBoxWithTexture(rect, texture);
            DisableAlphaChannel();
        }

        public static void DrawBoxWithAlpha(Box2D rect, Color color)
        {
            EnableAlphaChannel();
            GL.Color4(color);
            DrawBoxVertices(rect);
            DisableAlphaChannel();
        }


        private static void EnableAlphaChannel()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, OpenGL.BlendingFactorDest.OneMinusSrcAlpha);
        }

        private static void DisableAlphaChannel()
        {
            GL.Disable(OpenGL.EnableCap.Blend);
        }
    }
}
