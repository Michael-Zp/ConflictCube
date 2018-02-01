using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class Pickable : GameObject
    {
        public Pickable(string name, Transform transform, BoxCollider collider, Material material, GameObject parent) : base(name, transform, parent)
        {
            AddComponent(collider);
            AddComponent(material);
        }
    }
}
