using ConflictCube.Model.Tiles;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using ConflictCube.Model.Renderable;
using System;
using Zenseless.OpenGL;
using Zenseless.Geometry;
using ConflictCube.Controller;

namespace ConflictCube
{
    public class GameView
    {
        private MyWindow Window;
        private Dictionary<TileType, Texture> Tileset = new Dictionary<TileType, Texture>();

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

        public void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        

        public void Render(ViewModel viewModel)
        {
            RenderableLayer currentLayer;

            viewModel.RenderingLayers.TryGetValue(RenderLayerType.Floor, out currentLayer);
            RenderLayer(currentLayer, false);

            viewModel.RenderingLayers.TryGetValue(RenderLayerType.Player, out currentLayer);
            RenderLayer(currentLayer, true);
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

            foreach (RenderableObject currObj in currentLayer.ObjectsToRender)
            {
                Texture tempTexture;
                Tileset.TryGetValue(currObj.Type, out tempTexture);
                renderCall(currObj.Box, tempTexture);
            }
        }

        public void CloseWindow()
        {
            Window.Close();
        }
    }
}
