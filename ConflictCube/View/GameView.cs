using ConflictCube.Model.Tiles;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using ConflictCube.Model.Renderable;
using System;
using Zenseless.OpenGL;
using Zenseless.Geometry;
using ConflictCube.Controller;
using ConflictCube.Model.UI;
using ConflictCube.Model.Objects;
using OpenTK;

namespace ConflictCube
{
    public class GameView
    {
        private MyWindow Window;
        private Dictionary<TileType, Texture> Tileset = new Dictionary<TileType, Texture>();

        /// <summary>
        ///     Match the GL Viewport with the given window and load all Tilesets that are used in the game.
        /// </summary>
        /// <param name="window">The window of the game</param>
        public GameView(MyWindow window)
        {
            Window = window;

            Window.Resize += (s, a) => GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.ClearColor(Color4.CornflowerBlue);

            List<Dictionary<TileType, Texture>> tilesets = new List<Dictionary<TileType, Texture>>();
            tilesets.Add(TilesetLoader.LoadTileset(TilesetType.Floor));
            tilesets.Add(TilesetLoader.LoadTileset(TilesetType.Player));

            foreach (var tileset in tilesets)
            {
                foreach (var key in tileset.Keys)
                {
                    Texture tempTexture;
                    tileset.TryGetValue(key, out tempTexture);

                    Tileset.Add(key, tempTexture);
                }
            }
        }

        private void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        
        /// <summary>
        ///     Render the given ViewModel. Right now the rendering order of the RenderingLayers is this:
        ///     1. Floor
        ///     2. Player
        ///     3. Player Throw/Use indicator
        ///     3. UI
        /// </summary>
        /// <param name="viewModel"></param>
        public void Render(ViewModel viewModel)
        {
            ClearScreen();

            RenderableLayer currentLayer;

            viewModel.RenderingLayers.TryGetValue(RenderLayerType.Floor, out currentLayer);
            RenderLayer(currentLayer, false);

            viewModel.RenderingLayers.TryGetValue(RenderLayerType.Player, out currentLayer);
            RenderLayer(currentLayer, true);

            viewModel.RenderingLayers.TryGetValue(RenderLayerType.ThrowUseIndicator, out currentLayer);
            RenderLayerWithAlphaColor(currentLayer);

            viewModel.RenderingLayers.TryGetValue(RenderLayerType.UI, out currentLayer);
            RenderUI(currentLayer);
        }

        private void RenderLayer(RenderableLayer currentLayer, bool alpha)
        {
            Action<Box2D, Texture> renderCall;
            if(!alpha)
            {
                renderCall = OpenTKWrapper.DrawBoxWithTexture;
            }
            else
            {
                renderCall = OpenTKWrapper.DrawBoxWithTextureAndAlphaChannel;
            }

            foreach (RenderableObject currObj in currentLayer.GetRenderableObjects())
            {
                Texture tempTexture;
                Tileset.TryGetValue(currObj.Type, out tempTexture);
                renderCall(currObj.Box, tempTexture);
            }
        }

        private void RenderLayerWithAlphaColor(RenderableLayer currentLayer)
        {
            foreach (RenderableObject currObj in currentLayer.GetRenderableObjects())
            {
                ColoredBox cBox = (ColoredBox)currObj;

                if(cBox.Alpha)
                {
                    OpenTKWrapper.DrawBoxWithAlpha(currObj.Box, cBox.Color);
                }
                else
                {
                    OpenTKWrapper.DrawBox(currObj.Box, cBox.Color);
                }
            }
        }


        private void RenderUI(RenderableLayer currentLayer)
        {
            foreach(RenderableObject obj in currentLayer.GetRenderableObjects())
            {
                if(obj is UIText)
                {
                    UIText text = (UIText)obj;
                    Console.WriteLine(text.Text);

                }
                else if (obj is Canvas)
                {
                    Canvas canvas = (Canvas)obj;
                    OpenTKWrapper.DrawBox(canvas.Box, canvas.Color);
                }
            }
        }


        public void CloseWindow()
        {
            Window.Close();
        }
        
    }
}
