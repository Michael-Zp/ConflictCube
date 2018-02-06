using System.Drawing;
using Engine.Components;

namespace ConflictCube.Objects
{
    public class ColoredBox : GameObject
    {
        public Color Color { get; set; }
        public bool Alpha { get; set; }

        public ColoredBox(string name, Transform transform, Material material, GameObject parent, string type = "ColoredBox", bool enabled = true) : base(name, transform, parent, type, enabled)
        {
            AddComponent(material);
        }
    }
}