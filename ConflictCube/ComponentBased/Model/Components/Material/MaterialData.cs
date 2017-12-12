using System.Drawing;
using Zenseless.Geometry;
using Zenseless.HLGL;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased.Components
{
    public class MaterialData
    {
        public ITexture Texture;
        public Box2D UVCoordinates;
        public Color Color;

        public MaterialData(ITexture texture, Box2D uvCoordinates, Color color)
        {
            Texture = texture;
            UVCoordinates = uvCoordinates;
            Color = color;
        }
    }
}
