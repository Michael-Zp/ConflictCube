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
        public IShader Shader;
        
        public MaterialData(ITexture texture, Box2D uvCoordinates, Color color, IShader shader)
        {
            Texture = texture;
            UVCoordinates = uvCoordinates;
            Color = color;
            Shader = shader;
        }
    }
}
