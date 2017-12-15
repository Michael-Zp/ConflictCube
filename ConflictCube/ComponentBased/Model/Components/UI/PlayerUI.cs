using System.Drawing;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.UI;

namespace ConflictCube.ComponentBased
{
    public class PlayerUI : GameObject
    {
        public Player Player { get; private set; }

        private GameObject BackgroundLayer;
        private GameObject ForegroundLayer;

        private Canvas PlayerHealth;

        private Canvas CubesInInventory;
        private TextField CountCubesInInventory;

        private static Material BackgroundMat = new Material(null, null, Color.White);
        private static Material PlayerAliveMat = new Material(null, null, Color.Green);
        private static Material PlayerDeadMat = new Material(null, null, Color.Red);
        private static Material InventoryMat = new Material(null, null, Color.Wheat);

        private static Material CubeMaterial = new Material(null, null, Color.White);
        private static Material GrayCubeMaterial = new Material(null, null, Color.Gray);


        public PlayerUI(string name, Player player, Transform uiArea, GameObject parent) : base(name, uiArea, parent, GameObjectType.UI)
        {
            Player = player;
            BackgroundLayer = new GameObject("BackgroundPlayerUI", new Transform(0, 0, 1, 1), this);
            ForegroundLayer = new GameObject("ForegroundPlayerUI", new Transform(0, 0, 1, 1), this);
            

            BackgroundLayer.AddChild(new Canvas("Background", new Transform(0, 0, 1, 1), this, BackgroundMat));

            ForegroundLayer.AddChild(new TextField("HealthText", new Transform(-.85f, .9f, .37f, .1f), "Health"));

            PlayerHealth = new Canvas("Health" + player.Name, new Transform(0, .85f, .8f, .05f), ForegroundLayer, PlayerAliveMat);
            ForegroundLayer.AddChild(PlayerHealth);

            ForegroundLayer.AddChild(new TextField("InventoryText", new Transform(-1f, .6f, .3f, .1f), "Inventory"));

            ForegroundLayer.AddChild(new Canvas("Inventory", new Transform(0, .3f, .8f, .3f), ForegroundLayer, InventoryMat));

            CubesInInventory = new Canvas("CubesInInventory", new Transform(0, .45f, .5f, .1f), ForegroundLayer, CubeMaterial);
            ForegroundLayer.AddChild(CubesInInventory);

            CountCubesInInventory = new TextField("CountCubesInInventory", new Transform(.2f, .3f, .5f, .1f), Player.Inventory.Cubes.ToString());
            ForegroundLayer.AddChild(CountCubesInInventory);

            AddChild(BackgroundLayer);
            AddChild(ForegroundLayer);
        }

        public override void OnUpdate()
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

            if(Player.Inventory.Cubes > 0)
            {
                CubesInInventory.RemoveComponent<Material>();
                CubesInInventory.AddComponent(CubeMaterial);
            }
            else
            {
                CubesInInventory.RemoveComponent<Material>();
                CubesInInventory.AddComponent(GrayCubeMaterial);
            }

            CountCubesInInventory.Text = Player.Inventory.Cubes.ToString();
        }
    }
}
