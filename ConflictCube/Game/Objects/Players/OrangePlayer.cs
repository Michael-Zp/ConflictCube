using ConflictCube.Debug;
using Engine.Components;
using OpenTK;
using System;
using System.Drawing;
using Zenseless.OpenGL;

namespace ConflictCube.Objects
{
    public class OrangePlayer : Player
    {
        private Material FiremanMaterial;
        private bool MaterialsAreInitialized;


        public OrangePlayer(string name, Floor currentFloor, GameObject parent)
            : base(name, currentFloor, parent, "PlayerFire", "PlayerFire", "Orange")
        {
            if(!MaterialsAreInitialized)
            {
                MaterialsAreInitialized = true;
                FiremanMaterial = new Material(Color.White, (Texture)Tilesets.Instance().FiremanSheet.Tex, Tilesets.Instance().FiremanSheet.CalcSpriteTexCoords(0));
            }

            Horizontal = "PlayerAxisPlayer1Horizontal";
            Vertical = "PlayerAxisPlayer1Vertical";
            Sprint = "PlayerOneSprint";
            HitBlock = "PlayerOneUse";
            SwitchPositionY = "PlayerOneSwitchPositionY";
            SwitchPositionXY = "PlayerOneSwitchPositionXY";
            SwitchPositionX = "PlayerOneSwitchPositionX";
            ActiveGamePad = 0;
            
            GetComponent<Collider>().IgnoreCollisionsWith.Add("PlayerIce");
            
            AddComponent(FiremanMaterial);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (DebugGame.Player2PrintPosition && Type.Equals("PlayerFire"))
            {
                Console.WriteLine(Transform.GetPosition(WorldRelation.Global));
            }

            if (DebugGame.PrintUseFieldPositionOrangePlayer)
            {
                Console.WriteLine(UseField.Transform.GetPosition(WorldRelation.Global));
                Console.WriteLine(UseField.Transform.GetSize(WorldRelation.Global));
            }
        }

        protected override void HitSelectedBlock()
        {
            if(UseFieldIsOnUsableField())
            {
                LevelTile tile = GetLevelTileOnCubeLayerOfSelectedField();
                tile.GetComponent<Material>().Enabled = false;
                tile.GetComponent<ParticleSystem>().Enabled = true;
                tile.GetComponent<Collider>().Enabled = false;
                tile.Type = "None";
                Destroy(tile, tile.GetComponent<ParticleSystem>().Lifetime);
            }
        }

        protected override bool UseFieldIsOnUsableField()
        {
            LevelTile useTile = GetLevelTileOnCubeLayerOfSelectedField();
            if (useTile.Type.Equals("OrangeBlock") && useTile.EnabledInHierachy)
            {
                return true;
            }
            return false;
        }

        public override void OnCollision(Collider other)
        {
            base.OnCollision(other);

            switch (other.Type)
            {
                case "BlueFloor":
                    Die(Name + " stepped into deadly water without rubber boots.");
                    break;
            }
        }

        public override void ResetPositionToLastCheckpoint()
        {
            Transform.SetPosition(CurrentFloor.OrangePlayerCheckpoint.GetPosition(WorldRelation.Global), WorldRelation.Global);
            Transform.SetRotation(0, WorldRelation.Global);
        }
    }
}
