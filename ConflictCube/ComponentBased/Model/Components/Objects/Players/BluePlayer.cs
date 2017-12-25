using ConflictCube.ComponentBased.Components;
using System;

namespace ConflictCube.ComponentBased
{
    public class BluePlayer : Player
    {
        public BluePlayer(string name, Transform transform, BoxCollider boxCollider, Material material, GameObject parent, Floor currentFloor, float speed, GameObjectType playerType, bool isAlive = true)
            : base(name, transform, boxCollider, material, parent, currentFloor, speed, playerType, isAlive)
        {
            Horizontal = InputAxis.Player2Horizontal;
            Vertical = InputAxis.Player2Vertical;
            Sprint = InputKey.PlayerTwoSprint;
            HitBlock = InputKey.PlayerTwoUse;
            ActiveGamePad = 1;

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
            if (GetFloorTileOfUseField().Type == GameObjectType.BlueBlock)
            {
                GetFloorTileOfUseField().ChangeFloorTile(GameObjectType.Floor);
            }
        }
    }
}
