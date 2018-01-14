using System;
using System.Collections.Generic;
using System.Drawing;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.UI;
using ConflictCube.ResxFiles;
using OpenTK;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased
{
    public class PlayerUI : GameObject
    {
        public Player Player { get; private set; }

        private GameObject BackgroundLayer;
        private GameObject ForegroundLayer;

        private Canvas PlayerHealth;

        private Canvas BottomButton;
        private Canvas TopButton;
        private Canvas LeftButton;
        private Canvas RightButton;

        private Canvas SprintEnergyBackground;
        private List<Canvas> SprintEnergyBlocks = new List<Canvas>();
        private const int SprintBlockCount = 4;

        private static Material BackgroundMat = new Material(Color.White, null, null);
        private static Material PlayerAliveMat = new Material(Color.Green, null, null);
        private static Material PlayerDeadMat = new Material(Color.Red, null, null);
        
        private static Material SledgehammerMaterial;
        private static Material SwapMaterial;

        private static Material SprintEnergyBackgroundMaterial = new Material(Color.Black, null, null);
        private static Material SprintEnergyBlockMaterial = new Material(Color.Orange, null, null);

        private static bool IsInitalized = false;

        public PlayerUI(string name, Player player, Transform uiArea, GameObject parent) : base(name, uiArea, parent, GameObjectType.UI)
        {
            if(!IsInitalized)
            {
                SledgehammerMaterial = new Material(Color.White, Tilesets.Instance().InventoryTextures.Tex, new Zenseless.Geometry.Box2D(0, 0, 1, 1));
                SwapMaterial = new Material(Color.White, TextureLoader.FromBitmap(TexturResource.Swap), new Zenseless.Geometry.Box2D(0, 0, 1, 1));
            }

            Player = player;
            BackgroundLayer = new GameObject("BackgroundPlayerUI", new Transform(0, 0, 1, 1), this);
            ForegroundLayer = new GameObject("ForegroundPlayerUI", new Transform(0, 0, 1, 1), this);
            

            BackgroundLayer.AddChild(new Canvas("Background", new Transform(0, 0, 1, 1), this, BackgroundMat));

            ForegroundLayer.AddChild(new TextField("HealthText", new Transform(-.85f, .9f, .37f, .1f), "Health"));

            PlayerHealth = new Canvas("Health" + player.Name, new Transform(0, .85f, .8f, .05f), ForegroundLayer, PlayerAliveMat);
            ForegroundLayer.AddChild(PlayerHealth);

            // Abilities

            BottomButton = new Canvas("bottomAbility", new Transform(0, -.05f, .4f, .08f), ForegroundLayer, SledgehammerMaterial);
            TopButton = new Canvas("topAbility",       new Transform(0, .25f, .4f, .08f), ForegroundLayer, SwapMaterial);

            LeftButton   = new Canvas("leftAbility",   new Transform(-.5f, .1f, .4f, .08f), ForegroundLayer, SwapMaterial);
            RightButton  = new Canvas("rightAbility",  new Transform( .5f, .1f, .4f, .08f), ForegroundLayer, SwapMaterial);
            ForegroundLayer.AddChild(BottomButton);
            ForegroundLayer.AddChild(TopButton);
            ForegroundLayer.AddChild(LeftButton);
            ForegroundLayer.AddChild(RightButton);

            Transform topButtonTextTransform = (Transform)TopButton.Transform.Clone();
            Transform leftButtonTextTransform = (Transform)LeftButton.Transform.Clone();
            Transform rightButtonTextTransform = (Transform)RightButton.Transform.Clone();

            Vector2 textXYOffset = topButtonTextTransform.GetSize(WorldRelation.Global) / 2;
            textXYOffset.X += 0.008f;

            Vector2 textXorYOffset = leftButtonTextTransform.GetSize(WorldRelation.Global) / 2;
            textXorYOffset.X -= 0.005f;


            topButtonTextTransform.SetPosition(topButtonTextTransform.GetPosition(WorldRelation.Global) - textXYOffset, WorldRelation.Global);
            leftButtonTextTransform.SetPosition(leftButtonTextTransform.GetPosition(WorldRelation.Global) - textXorYOffset, WorldRelation.Global);
            rightButtonTextTransform.SetPosition(rightButtonTextTransform.GetPosition(WorldRelation.Global) - textXorYOffset, WorldRelation.Global);
            
            topButtonTextTransform.SetSize(topButtonTextTransform.GetSize(WorldRelation.Global) * .8f, WorldRelation.Global);
            leftButtonTextTransform.SetSize(leftButtonTextTransform.GetSize(WorldRelation.Global) * .8f, WorldRelation.Global);
            rightButtonTextTransform.SetSize(rightButtonTextTransform.GetSize(WorldRelation.Global) * .8f, WorldRelation.Global);

            ForegroundLayer.AddChild(new TextField("xySwap", topButtonTextTransform, "xy"));
            ForegroundLayer.AddChild(new TextField("xySwap", leftButtonTextTransform, "x"));
            ForegroundLayer.AddChild(new TextField("xySwap", rightButtonTextTransform, "y"));


            //Sprint Energy

            ForegroundLayer.AddChild(new TextField("Sprint", new Transform(-.8f, -.35f, .3f, .1f), "Sprint-"));
            ForegroundLayer.AddChild(new TextField("Energy", new Transform(-.8f, -.45f, .3f, .1f), "energy:"));

            SprintEnergyBackground = new Canvas("SprintEnergyBackground", new Transform(0, -.6f, .8f, .1f), ForegroundLayer, SprintEnergyBackgroundMaterial);
            ForegroundLayer.AddChild(SprintEnergyBackground);

            float blockHeight = SprintEnergyBackground.Transform.GetSize(WorldRelation.Local).Y * .9f;
            float blockWidht = (SprintEnergyBackground.Transform.GetSize(WorldRelation.Local).X * .8f) / SprintBlockCount;
            float marginWidht = (.9f - (SprintEnergyBackground.Transform.GetSize(WorldRelation.Local).X * .8f)) / (SprintBlockCount + 1);

            for (int i = 0; i < SprintBlockCount; i++)
            {
                float blockXPosition = SprintEnergyBackground.Transform.GetMinX(WorldRelation.Local) + marginWidht + blockWidht + (marginWidht * 1.5f + blockWidht * 2) * i;
                SprintEnergyBlocks.Add(new Canvas("SprintEnergyBlock" + i, new Transform(blockXPosition, SprintEnergyBackground.Transform.GetPosition(WorldRelation.Local).Y, blockWidht, blockHeight), ForegroundLayer, SprintEnergyBlockMaterial));
                ForegroundLayer.AddChild(SprintEnergyBlocks[i]);
            }


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

            float energyRatio = (float)Math.Floor((Player.CurrentSprintEnergy / Player.MaxSprintEnergy) * SprintBlockCount);

            for(int i = 0; i < SprintBlockCount; i++)
            {
                if(i < energyRatio)
                {
                    SprintEnergyBlocks[i].Enabled = true;
                }
                else
                {
                    SprintEnergyBlocks[i].Enabled = false;
                }
            }
        }
    }
}
