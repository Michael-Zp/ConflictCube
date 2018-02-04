using Engine.Components;

namespace ConflictCube.Objects
{
    class Boundary : GameObject 
    {
        public Boundary(string name, Transform transform, CollisionGroup group, string collType, GameObject parent, string type = "Boundary") : base(name, transform, parent, type)
        {
            AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, group, collType));
        }
    }
}
