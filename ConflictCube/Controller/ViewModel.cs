using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using ConflictCube.Model.UI;
using System.Collections.Generic;
using Zenseless.Geometry;
using ConflictCube.Model.Objects;
using System.Drawing;

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
            AddThrowUseUI(state);
            AddPlayerUI(state.PlayerUIs);
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

        private void AddThrowUseUI(GameState state)
        {
            RenderableLayer throwUseUiLayer = GeneratDefaultRenderableLayer();

            foreach (Player player in state.Players)
            {
                if (player.ThrowMode)
                {
                    Box2D throwUseBox = GetThrowUseBoxOfPlayer(player, state);
                    throwUseUiLayer.AddObjectsToRender(new ColoredBox(new Box2D(throwUseBox), Color.FromArgb(128, Color.Red), true));
                }
                else if (player.UseMode)
                {
                    Box2D throwUseBox = GetThrowUseBoxOfPlayer(player, state);
                    throwUseUiLayer.AddObjectsToRender(new ColoredBox(new Box2D(throwUseBox), Color.FromArgb(128, Color.Blue), true));
                }
            }

            RenderingLayers.Add(RenderLayerType.ThrowUseIndicator, throwUseUiLayer);
        }

        private Box2D GetThrowUseBoxOfPlayer(Player player, GameState state)
        {
            return state.CurrentLevel.GetBoxForGridOffsetOfPosition(player.ThrowUseField);
        }

        private void AddPlayerUI(PlayerUI[] uis)
        {
            RenderableLayer uiLayer = GeneratDefaultRenderableLayer();

            foreach(PlayerUI ui in uis)
            {
                uiLayer.AddRangedObjectsToRender(ui.GetRenderableObjects());
            }

            RenderingLayers.Add(RenderLayerType.UI, uiLayer);
        }
        

        private RenderableLayer GeneratDefaultRenderableLayer()
        {
            return new RenderableLayer(new List<RenderableObject>(), new List<RenderableLayer>(), new Box2D(-1, -1, 2, 2));
        }
    }
}
