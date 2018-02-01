using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class GhostPlayer : GameObject
    {
        private Player ActualPlayer;

        public GhostPlayer(string name, Transform transform, Material material, Player actualPlayer, GameObjectType type, GameObject parent) : base(name, transform, parent, type)
        {
            ActualPlayer = actualPlayer;

            AddComponent(material);
        }

        public override void OnUpdate()
        {
            Transform.SetPosition(ActualPlayer.Transform.GetPosition(WorldRelation.Local), WorldRelation.Local);
            Transform.SetSize(ActualPlayer.Transform.GetSize(WorldRelation.Local), WorldRelation.Local);
            //Rotation
        }
    }
}
