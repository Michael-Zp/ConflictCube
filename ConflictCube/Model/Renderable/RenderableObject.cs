using Zenseless.OpenGL;
using Zenseless.Geometry;
using OpenTK;
using ConflictCube.Model.Tiles;

namespace ConflictCube.Model.Renderable
{
    public abstract class RenderableObject
    {
        private static int CurrentID = 0;
        public int ID { get; private set; }
        public Box2D Box { get; set; }
        public TileType Type { get; private set; }

        public RenderableObject(Box2D box, TileType type)
        {
            ID = CurrentID;
            Box = box;
            Type = type;

            CurrentID++;
        }

        public RenderableObject(Vector2 position, Vector2 size, TileType type)
        {
            ID = CurrentID;
            Box = new Box2D(position.X, position.Y, size.X, size.Y);
            Type = type;

            CurrentID++;
        }
    }
}
