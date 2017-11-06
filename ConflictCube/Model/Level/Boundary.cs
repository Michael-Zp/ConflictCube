using ConflictCube.Model.Renderable;
using Zenseless.Geometry;
using Zenseless.OpenGL;

namespace ConflictCube.Model
{
    class Boundary : ICollidable 
    {
        public Box2D Box { get; private set; }
        public CollisionType CollisionType { get; private set; }

        public Boundary(Box2D box, CollisionType type)
        {
            Box = box;
            CollisionType = type;
        }

        public void OnCollide(CollisionType type)
        {}
    }
}
