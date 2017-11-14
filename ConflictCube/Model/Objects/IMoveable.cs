using OpenTK;
using Zenseless.Geometry;

namespace ConflictCube.Model.Renderable
{
    public interface IMoveable
    {
        float Speed { get; }
        Vector2 MoveVectorThisIteration { get; set; }

        void SetPosition(Vector2 position);
        Vector2 GetPosition();
        bool CanMove();
    }


    public static class IMoveableMethodExtension
    {
        public static void Move(this IMoveable moveable, Vector2 moveVector)
        {
            if(moveable.CanMove())
            {
                moveable.MoveVectorThisIteration += moveVector;
            }
        }

        public static void MoveInstantly(this IMoveable moveable, Vector2 moveVector)
        {
            if (moveable is RenderableObject)
            {
                Vector2 center = new Vector2(((RenderableObject)moveable).Box.CenterX, ((RenderableObject)moveable).Box.CenterY);
                ((RenderableObject)moveable).SetPosition(center + moveVector);
            }
        }

        public static Box2D MoveThisIteration(this IMoveable moveable, ICollidable collidable)
        {
            Box2D box = new Box2D(0, 0, 0, 0)
            {
                SizeX = collidable.CollisionBox.SizeX,
                SizeY = collidable.CollisionBox.SizeY,

                MinX = collidable.CollisionBox.MinX + moveable.MoveVectorThisIteration.X,
                MinY = collidable.CollisionBox.MinY + moveable.MoveVectorThisIteration.Y
            };

            moveable.MoveInstantly(moveable.MoveVectorThisIteration);

            return box;
        }

        public static void ClearMoveVector(this IMoveable moveable)
        {
            moveable.MoveVectorThisIteration = new Vector2(0, 0);
        }
    }

}
