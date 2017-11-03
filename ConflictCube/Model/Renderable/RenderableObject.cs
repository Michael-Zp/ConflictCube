using Zenseless.OpenGL;
using Zenseless.Geometry;
using OpenTK;

namespace ConflictCube.Model.Renderable
{
    public abstract class RenderableObject
    {
        private static int CurrentID = 0;
        public int ID { get; private set; }
        public Box2D Box { get; private set; }
        public Texture Texture { get; private set; }

        public RenderableObject(Vector2 position, Vector2 size, Texture texture)
        {
            ID = CurrentID;
            Box = new Box2D(position.X, position.Y, size.X, size.Y);
            Texture = texture;

            CurrentID++;
        }
    }
}
