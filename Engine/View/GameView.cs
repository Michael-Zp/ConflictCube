using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System;
using System.Collections.Generic;
using Engine.Controler;
using Engine.Components;
using Engine.UI;

namespace Engine.View
{
    public class GameView
    {
        private MyWindow Window;
        private OpenTKWrapper OpenTKWrapper;

        /// <summary>
        ///     Match the GL Viewport with the given window and load all Tilesets that are used in the game.
        /// </summary>
        /// <param name="window">The window of the game</param>
        public GameView(MyWindow window)
        {
            OpenTKWrapper = OpenTKWrapper.Instance();
            Window = window;

            Window.Resize += (s, a) =>
            {
                GL.Viewport(0, 0, Window.Width, Window.Height);
                OpenTKWrapper.WindowHeight = Window.Height;
                OpenTKWrapper.WindowWidth = Window.Width;
            };

            OpenTKWrapper.WindowHeight = Window.Height;
            OpenTKWrapper.WindowWidth = Window.Width;

            GL.ClearColor(0, 0, 0, 0);
        }

        public void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        
        public void Render(ViewModel viewModel)
        {
            foreach(Camera camera in viewModel.Cameras)
            {
                //Scale camera to keep world propotions in the view
                Vector2 newRenderTargetAspectRatio;
                if(Window.Width > Window.Height)
                {
                    newRenderTargetAspectRatio = new Vector2((float)Window.Height / (float)Window.Width, 1);
                }
                else if(Window.Height > Window.Width)
                {
                    newRenderTargetAspectRatio = new Vector2(1, (float)Window.Width / (float)Window.Height);
                }
                else
                {
                    newRenderTargetAspectRatio = new Vector2(1);
                }

                camera.RenderTarget.SetSize(camera.OriginalRenderTargetSize * newRenderTargetAspectRatio, WorldRelation.Global);
            }

            ClearScreen();
            foreach(Camera camera in viewModel.Cameras)
            {
                camera.FBO.Activate();
                RenderGameObject(camera.Transform, camera.RootGameObject);
                camera.FBO.Deactivate();

                OpenTKWrapper.DrawBox(camera.RenderTarget, Color.White, camera.FBO.Texture, new Zenseless.Geometry.Box2D(-1, -1, 1, 1), null, null, new Transform(), true);

                camera.FBO.Activate();
                ClearScreen();
                camera.FBO.Deactivate();
            }

            foreach(Tuple<Transform, Color> debugObject in DebugDraws)
            {
                OpenTKWrapper.DrawBoxWithAlphaChannel((viewModel.Cameras[0].Transform * debugObject.Item1).GetGlobalRotatedRectangle(), debugObject.Item2);
            }

            DebugDraws.Clear();
        }
        

        public void RenderGameObject(Transform cameraTransform, GameObject currentObject)
        {
            if (currentObject == null || !currentObject.EnabledInHierachy)
            {
                return;
            }
            
            Transform globalTransform = currentObject.Transform.TransformToGlobal();

            List<Material> currentMats = currentObject.GetComponents<Material>();

            foreach(Material currentMat in currentMats)
            {
                if (currentMat != null && currentMat.Enabled)
                {
                    OpenTKWrapper.DrawBox(globalTransform, currentMat.Color, currentMat.Texture, currentMat.UVCoordinates, currentMat.Shader, currentMat, cameraTransform, true);
                }
            }


            if (currentObject is TextField text)
            {
                float scaleFactor = 2.625f;
                Vector2 position = globalTransform.GetPosition(WorldRelation.Global);
                Vector2 size = globalTransform.GetSize(WorldRelation.Global);
                OpenTKWrapper.PrintText(position.X, position.Y, size.X, size.Y, text.Text, text.Font);
                globalTransform.SetSize(globalTransform.GetSize(WorldRelation.Global) / scaleFactor, WorldRelation.Global);
            }


            foreach (GameObject child in currentObject.Children)
            {
                RenderGameObject(cameraTransform, child);
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
