using ConflictCube.ComponentBased.Components;
using System;

namespace ConflictCube.ComponentBased
{
    public class OrangePlayer : Player
    {
        public OrangePlayer(string name, Transform transform, BoxCollider boxCollider, Material material, GameObject parent, Floor currentFloor, float speed, GameObjectType playerType, bool isAlive = true)
            : base(name, transform, boxCollider, material, parent, currentFloor, speed, playerType, isAlive)
        {
            Horizontal = InputAxis.Player1Horizontal;
            Vertical = InputAxis.Player1Vertical;
            Sprint = InputKey.PlayerOneSprint;
            HitBlock = InputKey.PlayerOneUse;
            ActiveGamePad = 0;

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
    }
}
