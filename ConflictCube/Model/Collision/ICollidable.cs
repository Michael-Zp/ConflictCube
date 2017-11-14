using ConflictCube.Model.Collision;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ConflictCube.Model
{
    public enum CollisionType
    {
        LeftBoundary,
        RightBoundary,
        TopBoundary,
        BottomBoundary,
        Player,
        Finish,
        Wall,
        Hole,
        NonCollider
    }

    public interface ICollidable
    {
        CollisionGroup CollisionGroup { get; set; }
        Box2D CollisionBox { get; }
        CollisionType CollisionType { get; }
        HashSet<CollisionType> CollidesWith { get; }

        void OnCollide(ICollidable other);
    }
}
