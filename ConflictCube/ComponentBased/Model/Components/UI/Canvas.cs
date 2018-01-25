namespace ConflictCube.ComponentBased.Components
{
    public class Canvas : GameObject
    {
        public Canvas(string name, Transform transform, Material material, GameObject parent) : base(name, transform, parent, GameObjectType.UI)
        {
            AddComponent(material);
        }
    }
}
