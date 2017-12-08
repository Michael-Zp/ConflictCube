using System.Drawing;
using Zenseless.Geometry;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased.Components
{
    public class MaterialData
    {
        public Texture Texture;
        public Box2D UVCoordinates;
        public Color Color;

        public MaterialData(Texture texture, Box2D uvCoordinates, Color color)
        {
            Texture = texture;
            UVCoordinates = uvCoordinates;
            Color = color;
        }
    }
}
