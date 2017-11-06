using OpenTK;

namespace ConflictCube.Model.Renderable
{
    public interface IMoveable
    {
        float Speed { get; }

        void Move(Vector2 moveVector);
        void SetPosition(Vector2 position);
        Vector2 GetPosition();
    }
}
