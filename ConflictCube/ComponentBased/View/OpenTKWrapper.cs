using OpenTK.Graphics.OpenGL;
using OpenGL = OpenTK.Graphics.OpenGL;
using Zenseless.Geometry;
using System.Drawing;
using Zenseless.HLGL;
using static ConflictCube.ComponentBased.View.ZenselessWrapper;
using System.Runtime.InteropServices;
using System;
using ConflictCube.ComponentBased.Components;
using OpenTK;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased
{
    public class OpenTKWrapper
    {
        public Color StandardColor = Color.White;
        public float WindowHeight = 0;
        public float WindowWidth = 0;

        private VAO VertexArrayObject = new VAO(OpenTK.Graphics.OpenGL4.PrimitiveType.Quads);


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


        public void DrawBox(Components.Rectangle rect, Color color, ITexture texture, Box2D uVCoordinates, IShader shader, Material currentMat, bool alphaChannel = true)
        {
            if (alphaChannel)
            {
                EnableAlphaChannel();
            }
            
            GL.Color4(color);

            if (shader != null)
            {
                shader.Activate();

                GL.Uniform2(shader.GetResourceLocation(ShaderResourceType.Uniform, "iResolution"), WindowWidth, WindowHeight);
                GL.Uniform1(shader.GetResourceLocation(ShaderResourceType.Uniform, "iGlobalTime"), Time.Time.CurrentTime);

                foreach (Tuple<string, float> parameter in currentMat.ShaderParameters1D)
                {
                    GL.Uniform1(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, parameter.Item1), parameter.Item2);
                }

                foreach (Tuple<string, Vector2> parameter in currentMat.ShaderParameters2D)
                {
                    GL.Uniform2(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, parameter.Item1), parameter.Item2);
                }

                foreach (Tuple<string, Vector3> parameter in currentMat.ShaderParameters3D)
                {
                    GL.Uniform3(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, parameter.Item1), parameter.Item2);
                }

                foreach (Tuple<string, Vector4> parameter in currentMat.ShaderParameters4D)
                {
                    GL.Uniform4(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, parameter.Item1), parameter.Item2);
                }

                Vector2[] uvCoords = new Vector2[]
                {
                    Vector2.Zero,
                    Vector2.UnitX,
                    Vector2.One,
                    Vector2.UnitY
                };

                VertexArrayObject.SetAttribute((int)Material.VertexShaderAttributes.UvPosition, uvCoords, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, 2);
            }
            
            if (texture != null && uVCoordinates != null)
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

            if(shader != null)
            {
                shader.Deactivate();
            }
            

            GL.Color4(StandardColor);

            if (alphaChannel)
            {
                DisableAlphaChannel();
            }
        }

        public static int DrawnBoxes = 0;


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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Vertex
        {
            public float x, y;
            public float u, v;
            public float r, g, b, a;
        };

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

        public void PrintText(float xPos, float yPos, float xSize, float ySize, string text, MyTextureFont font)
        {
            EnableAlphaChannel();
            EnableTextures();

            float characterLength = xSize / text.Length;

            //Text iText is left/bottom aligned. With this calculation it will be approximately center alingend      
            font.PrintWithSize(xPos - (xSize * 3.8f / 10), yPos - (ySize * 3 / 5), 0f, characterLength, ySize, 1f, text);
            

            DisableTextures();
            DisableAlphaChannel();
        }


        private void EnableAlphaChannel()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
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
