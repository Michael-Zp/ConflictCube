using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ConflictCube.ComponentBased.Controller;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.UI;
using ConflictCube.ComponentBased.View;
using System.Drawing;
using System;
using System.Collections.Generic;
using Zenseless.OpenGL;
using Zenseless.HLGL;
using OpenTK;

namespace ConflictCube.ComponentBased
{
    public class GameView
    {
        private MyWindow Window;
        private OpenTKWrapper OpenTKWrapper = OpenTKWrapper.Instance();

        private FBO RenderFrameBufferObject = null;
        private bool WindowSizeChanged = false;

        /// <summary>
        ///     Match the GL Viewport with the given window and load all Tilesets that are used in the game.
        /// </summary>
        /// <param name="window">The window of the game</param>
        public GameView(MyWindow window)
        {
            Window = window;

            Window.Resize += (s, a) =>
            {
                GL.Viewport(0, 0, Window.Width, Window.Height);
                WindowSizeChanged = true;
            };
            GL.ClearColor(0, 0, 0, 0);
            RenderFrameBufferObject = new FBO(Texture2dGL.Create(Window.Width, Window.Height));
        }

        public void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        
        public void Render(ViewModel viewModel)
        {
            if(WindowSizeChanged)
            {
                WindowSizeChanged = false;

                foreach(Camera camera in viewModel.Cameras)
                {
                    if(!camera.IsUiCamera)
                    {
                        Vector2 cameraPos = camera.RenderTarget.GetPosition(WorldRelation.Global);
                        camera.RenderTarget = new Transform(cameraPos.X, cameraPos.Y, (float)Window.Height / (float)Window.Width, 1f);
                    }
                }
            }

            ClearScreen();
            foreach(Camera camera in viewModel.Cameras)
            {
                RenderGameObject(camera.Transform, camera.RootGameObject, camera.FBO);

                OpenTKWrapper.DrawBox(camera.RenderTarget.GetGlobalRotatedRectangel(), Color.White, camera.FBO.Texture, new Zenseless.Geometry.Box2D(-1, -1, 1, 1), true);

                camera.FBO.Activate();
                ClearScreen();
                camera.FBO.Deactivate();
            }

            foreach(Tuple<Transform, Color> debugObject in DebugDraws)
            {
                OpenTKWrapper.DrawBoxWithAlphaChannel((viewModel.Cameras[2].Transform * debugObject.Item1).GetGlobalRotatedRectangel(), debugObject.Item2);
            }

            DebugDraws.Clear();
        }

        public void RenderGameObject(Transform cameraTransform, GameObject currentObject, FBO targetFBO)
        {
            if (currentObject == null || !currentObject.EnabledInHierachy)
            {
                return;
            }
            
            RenderFrameBufferObject.Activate();
            
            ClearScreen();

            Transform globalTransformInCamera = cameraTransform * currentObject.Transform.TransformToGlobal();

            List<Material> currentMats = currentObject.GetComponents<Material>();

            foreach(Material currentMat in currentMats)
            {
                if (currentMat != null)
                {
                    if(currentMat.Texture != null)
                    {
                        OpenTKWrapper.DrawBox(new Transform().GetGlobalNotRotatedRectangle(), currentMat.Color, currentMat.Texture, currentMat.UVCoordinates, true);
                    }
                    else
                    {
                        OpenTKWrapper.DrawBox(new Transform().GetGlobalNotRotatedRectangle(), currentMat.Color, null, null, true);
                    }
                }
            }

            //TODO: Text is not working anymore since the shader change.
            if(currentObject is TextField)
            {
                TextField text = (TextField)currentObject;
                Transform globalTransform = globalTransformInCamera;

                OpenTKWrapper.PrintText(globalTransform.GetPosition(WorldRelation.Global).X, globalTransform.GetPosition(WorldRelation.Global).Y, globalTransform.GetSize(WorldRelation.Global).X, globalTransform.GetSize(WorldRelation.Global).Y, text.Text);
            }

            //Apply shaders
            foreach (Material currentMat in currentMats)
            {
                if (currentMat.Shader != null)
                {
                    currentMat.Shader.Activate();
                    RenderFrameBufferObject.Texture.Activate();

                    GL.Uniform2(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, "iResolution"), (float)Window.Width, (float)Window.Height);
                    GL.Uniform1(currentMat.Shader.GetResourceLocation(ShaderResourceType.Uniform, "iGlobalTime"), Time.Time.CurrentTime);

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

                    GL.DrawArrays(PrimitiveType.Quads, 0, 4);

                    RenderFrameBufferObject.Texture.Deactivate();
                    currentMat.Shader.Deactivate();
                }
            }
            

            RenderFrameBufferObject.Deactivate();

            targetFBO.Activate();
            

            OpenTKWrapper.DrawBox(globalTransformInCamera.GetGlobalRotatedRectangel(), Color.White, RenderFrameBufferObject.Texture, new Zenseless.Geometry.Box2D(-1, -1, 1, 1), true);

            targetFBO.Deactivate();

            foreach (GameObject child in currentObject.Children)
            {
                RenderGameObject(cameraTransform, child, targetFBO);
            }          
        }

        public void CloseWindow()
        {
            Window.Close();
        }


        private static List<Tuple<Transform, Color>> DebugDraws = new List<Tuple<Transform, Color>>();

        public static void DrawDebug(Transform transform, Color color)
        {
            DebugDraws.Add(Tuple.Create(transform, color));
        }
        
    }
}
