using ConflictCube.Debug;
using ConflictCube.ResxFiles;
using Engine.Components;
using OpenTK;
using System;
using System.Drawing;
using Zenseless.OpenGL;

namespace ConflictCube.Objects
{
    public class BluePlayer : Player
    {
        private Material WelderMaterial;
        private bool MaterialsAreInitialized;

        public BluePlayer(string name, Floor currentFloor, GameObject parent)
            : base(name, currentFloor, parent, "PlayerIce", "PlayerIce", "Blue")
        {
            if (!MaterialsAreInitialized)
            {
                MaterialsAreInitialized = true;
                WelderMaterial = new Material(Color.White, (Texture)Tilesets.Instance().WelderSheet.Tex, Tilesets.Instance().WelderSheet.CalcSpriteTexCoords(0));

            }

            Horizontal = "PlayerAxisPlayer2Horizontal";
            Vertical = "PlayerAxisPlayer2Vertical";
            Sprint = "PlayerTwoSprint";
            HitBlock = "PlayerTwoPlayerTwoUse";
            SwitchPositionY = "PlayerTwoPlayerTwoSwitchPositionsY";
            SwitchPositionXY = "PlayerTwoPlayerTwoSwitchPositionsXY";
            SwitchPositionX = "PlayerTwoPlayerTwoSwitchPositionsX";
            ActiveGamePad = 1;

            GetComponent<Collider>().IgnoreCollisionsWith.Add("PlayerFire");

            AddComponent(WelderMaterial);
        }
            
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (DebugGame.Player1PrintPosition && Type.Equals("PlayerIce"))
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
                tile.Type = "None";
                Destroy(tile, tile.GetComponent<ParticleSystem>().Lifetime);



                Transform fireTransform = (Transform)Transform.Clone();
                fireTransform.SetPosition(fireTransform.GetPosition(WorldRelation.Global) + fireTransform.Forward * (fireTransform.GetSize(WorldRelation.Global).Length * 1.2f), WorldRelation.Global);
                GameObject fireObject = new GameObject("Fire", fireTransform, Parent);

                Material fireMat = new Material(Color.FromArgb(180, Color.White), TextureLoader.FromBitmap(ParticleSystemResources.WelderTorch), new Zenseless.Geometry.Box2D(Zenseless.Geometry.Box2D.BOX01));
                Func<float, float> sizeOverTime = new Func<float, float>((r) => { return 1 + 0.1f * (float)Math.Sin(8 * r); });
                Func<float, float> velocityOverTime = new Func<float, float>((r) => { return 0; });

                ParticleSystem sys = new ParticleSystem(1, 0, fireMat, new Vector2(1), .5f, sizeOverTime, velocityOverTime, new Vector2(0), 1);
                fireObject.AddComponent(sys);

                AudioPlayer destroySound = new AudioPlayer(AudioResources.StoneBreaking, false);
                fireObject.AddComponent(destroySound);
                destroySound.PlayAudio();

                Destroy(fireObject, .5f);
            }
        }

        protected override bool UseFieldIsOnUsableField()
        {
            LevelTile useTile = GetLevelTileOnCubeLayerOfSelectedField();
            if (useTile.Type.Equals("BlueBlock") && useTile.EnabledInHierachy)
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
                case "OrangeFloor":
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
