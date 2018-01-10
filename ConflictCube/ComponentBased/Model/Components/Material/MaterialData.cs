using System.Drawing;
using Zenseless.Geometry;
using Zenseless.HLGL;

namespace ConflictCube.ComponentBased.Components
{
    public class MaterialData
    {
        public ITexture Texture;
        public Box2D UVCoordinates;
        public Color Color;
        public string ShaderText;
        
        public MaterialData(ITexture texture, Box2D uvCoordinates, Color color, string shaderText)
        {
            Texture = texture;
            UVCoordinates = uvCoordinates;
            Color = color;
            ShaderText = shaderText;
        }
    }
}
