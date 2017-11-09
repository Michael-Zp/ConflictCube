using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using System.Collections.Generic;

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


        private void SetFloor(Level currentLevel)
        {
            if (RenderingLayers.ContainsKey(RenderLayerType.Floor))
            {
                RenderingLayers.Remove(RenderLayerType.Floor);
            }


            RenderableLayer floorLayer = new RenderableLayer();

            floorLayer.ObjectsToRender.AddRange(currentLevel.ObjectsToRender);

            RenderingLayers.Add(RenderLayerType.Floor, floorLayer);
        }

        private void AddPlayers(List<Player> players)
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

            playerLayer.ObjectsToRender.AddRange(players);

        }
    }
}
