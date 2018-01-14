using ConflictCube.ComponentBased.Components;
using OpenTK;
using System;
using System.Drawing;

namespace ConflictCube.ComponentBased
{
    public class OrangePlayer : Player
    {
        public OrangePlayer(string name, Transform transform, BoxCollider boxCollider, Material material, GameObject parent, Floor currentFloor, float speed, GameObjectType playerType, Player otherPlayer, bool isAlive = true)
            : base(name, transform, boxCollider, material, parent, currentFloor, speed, playerType, otherPlayer, isAlive)
        {
            Horizontal = InputAxis.Player1Horizontal;
            Vertical = InputAxis.Player1Vertical;
            Sprint = InputKey.PlayerOneSprint;
            HitBlock = InputKey.PlayerOneUse;
            SwitchPositionY = InputKey.PlayerOneSwitchPositionsY;
            SwitchPositionXY = InputKey.PlayerOneSwitchPositionsXY;
            SwitchPositionX = InputKey.PlayerOneSwitchPositionsX;
            ActiveGamePad = 0;

            AfterglowMaterialX.AddShaderParameter("desiredColor", new Vector3(Color.Orange.R, Color.Orange.G, Color.Orange.B));
            AfterglowMaterialY.AddShaderParameter("desiredColor", new Vector3(Color.Orange.R, Color.Orange.G, Color.Orange.B));
            AfterglowMaterialXY.AddShaderParameter("desiredColor", new Vector3(Color.Orange.R, Color.Orange.G, Color.Orange.B));


            GetComponent<Collider>().Layer = Model.Components.Colliders.CollisionLayer.Orange;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (DebugGame.Player2PrintPosition && Type == GameObjectType.PlayerFire)
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
            if(GetFloorTileOfUseField().Type == GameObjectType.OrangeBlock)
            {
                GetFloorTileOfUseField().ChangeFloorTile(GameObjectType.Floor);
            }
        }

        
        public override void OnCollision(Collider other)
        {
            base.OnCollision(other);

            switch (other.Type)
            {
                case CollisionType.BlueFloor:
                    Die();
                    break;
            }
        }
    }
}
