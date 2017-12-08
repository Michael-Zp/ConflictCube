using System.Drawing;
using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    public class PlayerUI : GameObject
    {
        public Player Player { get; private set; }

        private GameObject BackgroundLayer;
        private GameObject ForegroundLayer;

        private Canvas PlayerHealth;
        private static Material BackgroundMat = new Material(null, null, Color.White);
        private static Material PlayerAliveMat = new Material(null, null, Color.Green);
        private static Material PlayerDeadMat = new Material(null, null, Color.Red);


        public PlayerUI(string name, Player player, Transform uiArea, GameObject parent) : base(name, uiArea, parent, GameObjectType.UI)
        {
            Player = player;
            BackgroundLayer = new GameObject("BackgroundPlayerUI", new Transform(0, 0, 2, 2), this);
            ForegroundLayer = new GameObject("ForegroundPlayerUI", new Transform(0, 0, 2, 2), this);
            

            BackgroundLayer.AddChild(new Canvas("Canvas", new Transform(-1, -1, 2, 2), this, BackgroundMat));
            
            ForegroundLayer.AddChild(new Canvas("Canvas", new Transform(-1, -1, 2, 2), this, PlayerAliveMat));
        }

        public void UpdateUi()
        {
            if (Player.IsAlive)
            {
                PlayerHealth.RemoveComponent<Material>();
                PlayerHealth.AddComponent(PlayerAliveMat);
            }
            else
            {
                PlayerHealth.RemoveComponent<Material>();
                PlayerHealth.AddComponent(PlayerDeadMat);
            }
        }
    }
}
