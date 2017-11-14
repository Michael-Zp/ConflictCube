using OpenTK;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ConflictCube.Model
{
    class Boundary : ICollidable 
    {
        public Box2D CollisionBox { get; private set; }
        public CollisionType CollisionType { get; private set; }
        public HashSet<CollisionType> CollidesWith { get; }

        public Boundary(Box2D box, CollisionType type)
        {
            CollisionBox = box;
            CollisionType = type;
        }

        public void OnCollide(ICollidable other)
        {}

        public Boundary Clone()
        {
            Boundary clone = (Boundary)this.MemberwiseClone();
            clone.CollisionBox = new Box2D(CollisionBox);
            return clone;
        }
    }
}
