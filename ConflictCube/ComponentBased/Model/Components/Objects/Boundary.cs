using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    class Boundary : GameObject 
    {
        public Boundary(string name, Transform transform, CollisionGroup group, CollisionType collType, GameObject parent) : base(name, transform, parent, GameObjectType.Boundary)
        {
            AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, group, collType));
        }
    }
}
