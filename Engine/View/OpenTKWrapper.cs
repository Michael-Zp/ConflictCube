using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL = OpenTK.Graphics.OpenGL;
using Zenseless.Geometry;
using Zenseless.HLGL;
using Zenseless.OpenGL;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Engine.Components;
using static Engine.View.ZenselessWrapper;
using System.ComponentModel.Composition;
using Engine.Time;

namespace Engine.View
{
    public class OpenTKWrapper
    {
#pragma warning disable 0649

        [Import(typeof(ITime))]
        private ITime Time;

#pragma warning restore 0649

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

        public OpenTKWrapper()
        {
            GameEngine.Container.ComposeParts(this);
        }

        public void DrawBoxWithAlphaChannel(Components.Rectangle rect, Color color)
        {
            EnableAlphaChannel();

            GL.Color4(color);

            DrawBox(rect);

            GL.Color4(StandardColor);

            DisableAlphaChannel();
        }


        public void DrawBox(Transform globalTransform, Color color, ITexture texture, Box2D uVCoordinates, IShader shader, Material currentMat, Transform cameraTransform, bool alphaChannel = true)
        {
            var rect = globalTransform.GetGlobalRotatedRectangle();


            if (alphaChannel)
            {
                EnableAlphaChannel();
            }
            
            GL.Color4(color);

            if (shader != null)
            {
                shader.Activate();

                GL.Uniform2(shader.GetResourceLocation(ShaderResourceType.Uniform, "iResolution"), WindowWidth, WindowHeight);
                GL.Uniform1(shader.GetResourceLocation(ShaderResourceType.Uniform, "iGlobalTime"), Time.CurrentTime);

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

                Vector2[] uvCoords;

                if (texture != null && uVCoordinates != null)
                {
                    GL.Uniform1(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, "tex"), texture.ID);
                    uvCoords = new Vector2[]
                    {
                        new Vector2(uVCoordinates.MinX, uVCoordinates.MinY),
                        new Vector2(uVCoordinates.MaxX, uVCoordinates.MinY),
                        new Vector2(uVCoordinates.MaxX, uVCoordinates.MaxY),
                        new Vector2(uVCoordinates.MinX, uVCoordinates.MaxY),
                    };

                    GL.Uniform1(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, "minXuv"), uVCoordinates.MinX);
                    GL.Uniform1(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, "maxXuv"), uVCoordinates.MaxX);
                    GL.Uniform1(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, "minYuv"), uVCoordinates.MinY);
                    GL.Uniform1(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, "maxYuv"), uVCoordinates.MaxY);
                }
                else
                {
                    uvCoords = new Vector2[]
                    {
                        Vector2.Zero,
                        Vector2.UnitX,
                        Vector2.One,
                        Vector2.UnitY
                    };
                }

                int uvPos = currentMat.Shader.GetResourceLocation(ShaderResourceType.Attribute, "uvPos");

                VertexArrayObject.SetAttribute(uvPos, uvCoords, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, 2);
                
                float minX = cameraTransform.GetMinX(WorldRelation.Global);
                float maxX = cameraTransform.GetMaxX(WorldRelation.Global);
                float minY = cameraTransform.GetMinY(WorldRelation.Global);
                float maxY = cameraTransform.GetMaxY(WorldRelation.Global);
                
                Matrix4 cameraMatrix = System.Numerics.Matrix4x4.CreateOrthographicOffCenter(minX, maxX, minY, maxY, 0, 1).ToOpenTK().Inverted();
                
                int locCamera = currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, "camera");

                GL.UniformMatrix4(locCamera, false, ref cameraMatrix);

                Vector2[] positions = new Vector2[]
                {
                    rect.BottomLeft,
                    rect.BottomRight,
                    rect.TopRight,
                    rect.TopLeft
                };

                int posPos = currentMat.Shader.GetResourceLocation(ShaderResourceType.Attribute, "position");

                VertexArrayObject.SetAttribute(posPos, positions, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, 2);
            }

            if (texture != null && uVCoordinates != null)
            {
                EnableTextures();
                texture.Activate();
            }


            if(shader != null)
            {
                VertexArrayObject.Draw();
            }
            else
            {
                rect = rect.ApplyTransform(cameraTransform);

                if (texture != null && uVCoordinates != null)
                {
                    DrawTexturedBox(rect, uVCoordinates);
                }
                else
                {
                    DrawBox(rect);
                }
            }

            if(texture != null && uVCoordinates != null)
            { 
                texture.Deactivate();
                DisableTextures();
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
