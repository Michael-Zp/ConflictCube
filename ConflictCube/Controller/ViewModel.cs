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
            AddPlayer(state.Player);
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

            floorLayer.ObjectsToRender.AddRange(currentLevel.FloorLeft.ObjectsToRender);
            floorLayer.ObjectsToRender.AddRange(currentLevel.FloorMiddle.ObjectsToRender);
            floorLayer.ObjectsToRender.AddRange(currentLevel.FloorRight.ObjectsToRender);

            RenderingLayers.Add(RenderLayerType.Floor, floorLayer);
        }

        private void AddPlayer(Player player)
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
    }
}
