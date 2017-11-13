using OpenTK;

namespace ConflictCube.Model.Renderable
{
    public interface IMoveable
    {
        float Speed { get; }
        Vector2 MoveVectorThisIteration { get; set; }

        void SetPosition(Vector2 position);
        Vector2 GetPosition();
    }


    public static class IMoveableMethodExtension
    {
        public static void Move(this IMoveable moveable, Vector2 moveVector)
        {
            moveable.MoveVectorThisIteration += moveVector;
        }
    }

}
