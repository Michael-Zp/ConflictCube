using ConflictCube.Model.Tiles;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using ConflictCube.Model.Renderable;
using System;

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

        public void Render()
        {
            RenderableLayer currentLayer;

            RenderingLayers.TryGetValue(RenderLayerType.Floor, out currentLayer);
            RenderLayer(currentLayer);

            RenderingLayers.Clear();
        }

        private void RenderLayer(RenderableLayer currentLayer)
        {
            foreach(RenderableObject currObj in currentLayer.ObjectsToRender)
            {
                OpenTKWrapper.DrawBoxWithTexture(currObj.Box, currObj.Texture);
            }

        }

        public void CloseWindow()
        {
            Window.Close();
        }
    }
}
