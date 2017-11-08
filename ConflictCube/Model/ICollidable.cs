using OpenTK;
using System;
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
        Box2D CollisionBox { get; }
        CollisionType CollisionType { get; }
        HashSet<CollisionType> CollidesWith { get; }

        void OnCollide(CollisionType type, ICollidable other, Vector2 movementIntoCollision);
    }
}
