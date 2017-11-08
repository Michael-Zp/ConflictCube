using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;

namespace ConflictCube
{
    public class Player : RenderableObject, IMoveable, ICollidable
    {
        public float Speed { get; private set; }
        public bool IsAlive { get; private set; }
        public CollisionType CollisionType { get; private set; }


        public Player(Vector2 size, Vector2 position, float speed, bool isAlive = true) : base(position, size, TileType.Player)
        {
            Speed = speed;
            IsAlive = isAlive;
            CollisionType = CollisionType.Player;
        }

        public void Move(Vector2 moveVector)
        {
            Box.CenterX += moveVector.X;
            Box.CenterY += moveVector.Y;
        }

        public void SetPosition(Vector2 position)
        {
            Box.CenterX = position.X;
            Box.CenterY = position.Y;
        }

        public void OnCollide(CollisionType type)
        {
            switch (type)
            {
                case CollisionType.LeftBoundary:
                    Box.MinX = -1f;
                    break;

                case CollisionType.RightBoundary:
                    Box.MinX = 1f - Box.SizeX;
                    break;

                case CollisionType.TopBoundary:
                    Box.MinY = 1f - Box.SizeY;
                    break;

                case CollisionType.BottomBoundary:
                    IsAlive = false;
                    break;
            }
        }

        public Vector2 GetPosition()
        {
            return new Vector2(Box.CenterX, Box.CenterY);
        }
    }
}