using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ConflictCube.ComponentBased.Controller;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.UI;
using ConflictCube.ComponentBased.View;
using System.Drawing;
using System;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased
{
    public class GameView
    {
        private MyWindow Window;
        private OpenTKWrapper OpenTKWrapper = OpenTKWrapper.Instance();

        /// <summary>
        ///     Match the GL Viewport with the given window and load all Tilesets that are used in the game.
        /// </summary>
        /// <param name="window">The window of the game</param>
        public GameView(MyWindow window)
        {
            Window = window;

            Window.Resize += (s, a) => GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.ClearColor(Color4.CornflowerBlue);
        }

        private void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        
        public void Render(ViewModel viewModel)
        {
            ClearScreen();
            foreach(Camera camera in viewModel.Cameras)
            {
                RenderGameObject(camera.Transform, camera.RootGameObject);
            }

            foreach(Tuple<Transform, Color> debugObject in DebugDraws)
            {
                OpenTKWrapper.DrawBoxWithAlphaChannel(viewModel.Cameras[2].Transform * debugObject.Item1, debugObject.Item2);
            }

            DebugDraws.Clear();
        }

        private void RenderGameObject(Transform cameraTransform, GameObject currentObject)
        {
            if (!currentObject.EnabledInHierachy)
            {
                return;
            }

            Transform globalTransformInCamera = cameraTransform * currentObject.Transform.TransformToGlobal();

            List<Material> currentMats = currentObject.GetComponents<Material>();

            foreach(Material currentMat in currentMats)
            {
                if (currentMat != null)
                {
                    if (currentMat.Texture != null)
                    {
                        OpenTKWrapper.DrawBoxWithTextureAndAlphaChannel(globalTransformInCamera, currentMat.Texture, currentMat.UVCoordinates, currentMat.Color);
                    }
                    else
                    {
                        OpenTKWrapper.DrawBoxWithAlphaChannel(globalTransformInCamera, currentMat.Color);
                    }
                }
            }

            if(currentObject is TextField)
            {
                TextField text = (TextField)currentObject;
                Transform globalTransform = globalTransformInCamera;

                OpenTKWrapper.PrintText(globalTransform.GetPosition(WorldRelation.Global).X, globalTransform.GetPosition(WorldRelation.Global).Y, globalTransform.GetSize(WorldRelation.Global).X, globalTransform.GetSize(WorldRelation.Global).Y, text.Text);
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
