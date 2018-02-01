using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;
using ConflictCube.ComponentBased.Model.Components.Colliders;
using ConflictCube.ComponentBased.Model.Components.ParticleSystem;
using OpenTK;
using System;
using System.Drawing;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased
{
    public class BluePlayer : Player
    {
        private Material WelderMaterial;
        private bool MaterialsAreInitialized;

        public BluePlayer(string name, Floor currentFloor, GameObject parent)
            : base(name, currentFloor, parent, GameObjectType.PlayerIce, CollisionType.PlayerIce, CollisionLayer.Blue)
        {
            if (!MaterialsAreInitialized)
            {
                MaterialsAreInitialized = true;
                WelderMaterial = new Material(Color.White, (Texture)Tilesets.Instance().WelderSheet.Tex, Tilesets.Instance().WelderSheet.CalcSpriteTexCoords(0));

            }

            Horizontal = InputAxis.Player2Horizontal;
            Vertical = InputAxis.Player2Vertical;
            Sprint = InputKey.PlayerTwoSprint;
            HitBlock = InputKey.PlayerTwoUse;
            SwitchPositionY = InputKey.PlayerTwoSwitchPositionsY;
            SwitchPositionXY = InputKey.PlayerTwoSwitchPositionsXY;
            SwitchPositionX = InputKey.PlayerTwoSwitchPositionsX;
            ActiveGamePad = 1;

            AfterglowMaterialX.AddShaderParameter("desiredColor", new Vector3(Color.Blue.R, Color.Blue.G, Color.Blue.B));
            AfterglowMaterialY.AddShaderParameter("desiredColor", new Vector3(Color.Blue.R, Color.Blue.G, Color.Blue.B));
            AfterglowMaterialXY.AddShaderParameter("desiredColor", new Vector3(Color.Blue.R, Color.Blue.G, Color.Blue.B));

            GetComponent<Collider>().IgnoreCollisionsWith.Add(CollisionType.PlayerFire);

            AddComponent(WelderMaterial);
        }
            
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (DebugGame.Player1PrintPosition && Type == GameObjectType.PlayerIce)
            {
                Console.WriteLine(Transform.GetPosition(WorldRelation.Global));
            }
        }

        protected override void HitSelectedBlock()
        {
            if (UseFieldIsOnUsableField())
            {
                LevelTile tile = GetLevelTileOnCubeLayerOfSelectedField();
                tile.GetComponent<Material>().Enabled = false;
                tile.GetComponent<ParticleSystem>().Enabled = true;
                tile.GetComponent<Collider>().Enabled = false;
                tile.Type = GameObjectType.None;
                Destroy(tile, tile.GetComponent<ParticleSystem>().Lifetime);
            }
        }

        protected override bool UseFieldIsOnUsableField()
        {
            LevelTile useTile = GetLevelTileOnCubeLayerOfSelectedField();
            if (useTile.Type == GameObjectType.BlueBlock && useTile.EnabledInHierachy)
            {
                return true;
            }
            return false;
        }

        public override void OnCollision(Collider other)
        {
            base.OnCollision(other);

            switch(other.Type)
            {
                case CollisionType.OrangeFloor:
                    Die(Name + " stepped into lava without fire resistant boots.");
                    break;
            }
        }

        public override void ResetPositionToLastCheckpoint()
        {
            Transform.SetPosition(CurrentFloor.BluePlayerCheckpoint.GetPosition(WorldRelation.Global), WorldRelation.Global);
            Transform.SetRotation(0, WorldRelation.Global);
        }
    }
}
