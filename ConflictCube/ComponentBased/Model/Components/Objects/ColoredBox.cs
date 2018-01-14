using System.Drawing;
using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    public class ColoredBox : GameObject
    {
        public Color Color { get; set; }
        public bool Alpha { get; set; }

        public ColoredBox(string name, Transform transform, Material material, bool enabled = true) : base(name, transform, GameObjectType.ColoredBox, enabled)
        {
            AddComponent(material);
        }
    }
}
