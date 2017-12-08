namespace ConflictCube.ComponentBased.Components
{
    public class Canvas : GameObject
    {
        public Canvas(string name, Transform transform, GameObject parent, Material material) : base(name, transform, parent)
        {
            AddComponent(material);
        }
    }
}
