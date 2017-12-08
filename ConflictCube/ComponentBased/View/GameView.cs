using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ConflictCube.ComponentBased.Controller;
using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    public class GameView
    {
        private MyWindow Window;

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
            RenderGameObject(viewModel.Scene);
            RenderGameObject(viewModel.UI);            
        }

        private void RenderGameObject(GameObject currentObject)
        {
            Material currentMat = currentObject.GetComponent<Material>();
            if(currentMat != null)
            {
                OpenTKWrapper.DrawBoxWithTextureAndAlphaChannel(currentObject.Transform, currentMat.Texture, currentMat.UVCoordinates, currentMat.Color);
            }

            foreach (GameObject child in currentObject.Children)
            {
                RenderGameObject(currentObject);
            }
        }

        public void CloseWindow()
        {
            Window.Close();
        }
        
    }
}
