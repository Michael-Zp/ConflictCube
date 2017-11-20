using OpenTK;
using Zenseless.Geometry;

namespace ConflictCube.Model.Renderable
{
    public interface IMoveable
    {
        float Speed { get; }
        Vector2 MoveVectorThisIteration { get; set; }

        void Move(Vector2 moveVector);
        void SetPosition(Vector2 position);
        Vector2 GetPosition();
        bool CanMove();
    }


    public static class IMoveableMethodExtension
    {
        public static void MoveInstantly(this IMoveable moveable, Vector2 moveVector)
        {
            if (moveable is RenderableObject)
            {
                Vector2 center = new Vector2(((RenderableObject)moveable).Box.CenterX, ((RenderableObject)moveable).Box.CenterY);
                ((RenderableObject)moveable).SetPosition(center + moveVector);
            }
        }

        public static void MoveThisIteration(this IMoveable moveable)
        {
            moveable.MoveInstantly(moveable.MoveVectorThisIteration);
        }

        public static void ClearMoveVector(this IMoveable moveable)
        {
            moveable.MoveVectorThisIteration = new Vector2(0, 0);
        }
    }

}
