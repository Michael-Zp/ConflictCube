using Zenseless.OpenGL;
using Zenseless.Geometry;
using OpenTK;

namespace ConflictCube.Model.Renderable
{
    public abstract class RenderableObject
    {
        private static int CurrentID = 0;
        public int ID { get; private set; }
        public Box2D Box { get; set; }
        public Texture Texture { get; private set; }

        public RenderableObject(Box2D box, Texture texture)
        {
            ID = CurrentID;
            Box = box;
            Texture = texture;

            CurrentID++;
        }

        public RenderableObject(Vector2 position, Vector2 size, Texture texture)
        {
            ID = CurrentID;
            Box = new Box2D(position.X, position.Y, size.X, size.Y);
            Texture = texture;

            CurrentID++;
        }
    }
}
