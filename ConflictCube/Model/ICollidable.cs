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
        Hole,
        Wall,
        NoCollision
    }

    public interface ICollidable
    {
        CollisionType CollisionType { get; }

        void OnCollide(CollisionType type);
    }
}
