using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class GhostPlayer : GameObject
    {
        private Player ActualPlayer;

        public GhostPlayer(string name, Transform transform, GameObject parent, Material material, Player actualPlayer, GameObjectType type) : base(name, transform, parent, type)
        {
            ActualPlayer = actualPlayer;

            AddComponent(material);
        }

        public override void OnUpdate()
        {
            Transform.Position = ActualPlayer.Transform.Position;
            Transform.Size = ActualPlayer.Transform.Size;
            //Rotation
        }
    }
}
