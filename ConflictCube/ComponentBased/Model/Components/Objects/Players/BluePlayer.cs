using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;
using ConflictCube.ComponentBased.Model.Components.UI;
using OpenTK;
using System;
using System.Drawing;

namespace ConflictCube.ComponentBased
{
    public class BluePlayer : Player
    {
        public BluePlayer(string name, Transform transform, BoxCollider boxCollider, Material material, GameObject parent, Floor currentFloor, float speed, GameObjectType playerType, Player otherPlayer, bool isAlive = true)
            : base(name, transform, boxCollider, material, parent, currentFloor, speed, playerType, otherPlayer, isAlive)
        {
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

            GetComponent<Collider>().Layer = Model.Components.Colliders.CollisionLayer.Blue;
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
                GetLevelTileOnCubeLayerOfSelectedField().Enabled = false;
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

    }
}
