using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using System;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ConflictCube.Controller
{
    public class ViewModel
    {
        public Dictionary<RenderLayerType, RenderableLayer> RenderingLayers { get; set; }

        public ViewModel(GameState state)
        {
            RenderingLayers = new Dictionary<RenderLayerType, RenderableLayer>();
            SetState(state);
        }

        private void SetState(GameState state)
        {
            AddLevel(state.CurrentLevel);
            AddPlayers(state.Players);
        }


        private void AddLevel(Level currentLevel)
        {
            SetFloor(currentLevel);
        }

        private RenderableLayer GeneratDefaultRenderableLayer()
        {
            return new RenderableLayer(new List<RenderableObject>(), new List<RenderableLayer>(), new Box2D(-1, -1, 2, 2));
        }


        private void SetFloor(Level currentLevel)
        {
            if (RenderingLayers.ContainsKey(RenderLayerType.Floor))
            {
                RenderingLayers.Remove(RenderLayerType.Floor);
            }


            RenderableLayer floorLayer = GeneratDefaultRenderableLayer();

            floorLayer.AddRangedObjectsToRender(currentLevel.GetRenderableObjects());
            

            RenderingLayers.Add(RenderLayerType.Floor, floorLayer);
        }

        private void AddPlayers(List<Player> players)
        {
            RenderableLayer playerLayer;
            if (!RenderingLayers.ContainsKey(RenderLayerType.Player))
            {
                playerLayer = GeneratDefaultRenderableLayer();
                RenderingLayers.Add(RenderLayerType.Player, playerLayer);
            }
            else
            {
                RenderingLayers.TryGetValue(RenderLayerType.Player, out playerLayer);
            }

            playerLayer.AddRangedObjectsToRender(players);

        }
    }
}
