using OpenTK;
using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using Zenseless.Geometry;
using System.Drawing;

namespace ConflictCube.Model.Objects
{
    public class ColoredBox : RenderableObject
    {
        public Color Color { get; set; }
        public bool Alpha { get; set; }

        public ColoredBox(Box2D box, Color color, bool alpha) : base(box, TileType.ColoredBox)
        {
            Color = color;
            Alpha = alpha;
        }

        public override void OnBoxChanged()
        {}

        public override void SetPosition(Vector2 pos)
        {
            Box.CenterX = pos.X;
            Box.CenterY = pos.Y;
        }
    }
}
