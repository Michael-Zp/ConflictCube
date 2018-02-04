using Engine.Components;

namespace Engine.UI
{
    public class Canvas : GameObject
    {
        public Canvas(string name, Transform transform, Material material, GameObject parent, string type = "Canvas") : base(name, transform, parent, type)
        {
            AddComponent(material);
        }
    }
}
