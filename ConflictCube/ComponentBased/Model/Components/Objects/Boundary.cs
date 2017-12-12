using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    class Boundary : GameObject 
    {
        public Boundary(string name, Transform transform, GameObject parent, CollisionGroup group, CollisionType collType) : base(name, transform, parent, GameObjectType.Boundary)
        {
            AddComponent(new BoxCollider(transform, false, group, collType));
        }
    }
}
