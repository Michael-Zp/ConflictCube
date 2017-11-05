using ConflictCube.Model.Tiles;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using ConflictCube.Model.Renderable;
using System;
using Zenseless.OpenGL;
using Zenseless.Geometry;

namespace ConflictCube
{
    public class GameView
    {
        private MyWindow Window;
        private Dictionary<RenderLayerType, RenderableLayer> RenderingLayers = new Dictionary<RenderLayerType, RenderableLayer>();

        public GameView(MyWindow window)
        {
            Window = window;

            Window.Resize += (s, a) => GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.ClearColor(Color4.CornflowerBlue);
        }

        internal void SetLevel(Level currentLevel)
        {
            SetFloorRenderingLayer(currentLevel.Floor);
        }

        public void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        
        public void SetFloorRenderingLayer(Floor currentFloor)
        {
            if (RenderingLayers.ContainsKey(RenderLayerType.Floor))
            {
                RenderingLayers.Remove(RenderLayerType.Floor);
            }
            

            RenderableLayer floorLayer = new RenderableLayer();

            floorLayer.ObjectsToRender.AddRange(currentFloor.ObjectsToRender);

            RenderingLayers.Add(RenderLayerType.Floor, floorLayer);
        }

        public void AddPlayer(Player player)
        {
            RenderableLayer playerLayer;
            if (!RenderingLayers.ContainsKey(RenderLayerType.Player))
            {
                playerLayer = new RenderableLayer();
                RenderingLayers.Add(RenderLayerType.Player, playerLayer);
            }
            else
            {
                RenderingLayers.TryGetValue(RenderLayerType.Player, out playerLayer);
            }

            playerLayer.ObjectsToRender.Add(player);
        }

        public void Render()
        {
            RenderableLayer currentLayer;

            RenderingLayers.TryGetValue(RenderLayerType.Floor, out currentLayer);
            RenderLayer(currentLayer, false);

            RenderingLayers.TryGetValue(RenderLayerType.Player, out currentLayer);
            RenderLayer(currentLayer, true);

            RenderingLayers.Clear();
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
                renderCall(currObj.Box, currObj.Texture);
            }
        }

        public void CloseWindow()
        {
            Window.Close();
        }
    }
}
