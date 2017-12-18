using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ConflictCube.ComponentBased.Controller;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.UI;

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

        
        /// <summary>
        ///     Render the given ViewModel. Right now the rendering order is:
        ///     1. Scene
        ///     2. UI
        /// </summary>
        /// <param name="viewModel"></param>
        public void Render(ViewModel viewModel)
        {
            ClearScreen();
            RenderGameObject(viewModel.Game);         
        }

        private void RenderGameObject(GameObject currentObject)
        {
            if (!currentObject.EnabledInHierachy)
            {
                return;
            }

            Material currentMat = currentObject.GetComponent<Material>();
            if(currentMat != null)
            {
                if(currentMat.Texture != null)
                {
                    OpenTKWrapper.DrawBoxWithTextureAndAlphaChannel(currentObject.Transform.TransformToGlobal(), currentMat.Texture, currentMat.UVCoordinates, currentMat.Color);
                }
                else
                {
                    OpenTKWrapper.DrawBoxWithAlphaChannel(currentObject.Transform.TransformToGlobal(), currentMat.Color);
                }
            }

            if(currentObject is TextField)
            {
                TextField text = (TextField)currentObject;
                Transform globalTransform = text.Transform.TransformToGlobal();

                OpenTKWrapper.PrintText(globalTransform.Position.X, globalTransform.Position.Y, globalTransform.Size.X, globalTransform.Size.Y, text.Text);
            }

            foreach (GameObject child in currentObject.Children)
            {
                RenderGameObject(child);
            }
        }

        public void CloseWindow()
        {
            Window.Close();
        }
        
    }
}
