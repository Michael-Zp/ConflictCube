using System.Collections.Generic;
using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;
using Zenseless.Geometry;
using System;
using ConflictCube.Model.Collision;

namespace ConflictCube
{
    public class Player : RenderableObject, IMoveable, ICollidable
    {
        public Box2D CollisionBox { get; private set; }
        public float Speed { get; private set; }
        public bool IsAlive { get; private set; }
        public CollisionType CollisionType { get; private set; }
        public HashSet<CollisionType> CollidesWith { get; private set; }
        public Vector2 MoveVectorThisIteration { get; set; }

        public CollisionGroup CollisionGroup { get; set; }

        public Player(Vector2 size, Vector2 position, float speed, bool isAlive = true) : base(position, size, TileType.Player)
        {
            CollisionBox = Box;
            Speed = speed;
            IsAlive = isAlive;
            InitializeCollision();
        }

        private void InitializeCollision()
        {
            CollisionType = CollisionType.Player;
            CollidesWith = new HashSet<CollisionType>
            {
                CollisionType.LeftBoundary,
                CollisionType.RightBoundary,
                CollisionType.TopBoundary,
                CollisionType.BottomBoundary,
                CollisionType.Player,
                CollisionType.Finish,
                CollisionType.Wall,
                CollisionType.Hole
            };
        }

        public override void SetPosition(Vector2 position)
        {
            Box.CenterX = position.X;
            Box.CenterY = position.Y;
            OnBoxChanged();
        }

        private static int CollisionCount = 0;

        public void OnCollide(ICollidable other)
        {
            if (other.CollisionType == CollisionType.LeftBoundary || other.CollisionType == CollisionType.RightBoundary || other.CollisionType == CollisionType.TopBoundary || other.CollisionType == CollisionType.Wall )
            {
                CollisionCount++;
                Console.WriteLine(CollisionCount);

            }
            else if (other.CollisionType == CollisionType.BottomBoundary || other.CollisionType == CollisionType.Hole)
            {
                IsAlive = false;
            }
            else if (other.CollisionType == CollisionType.Finish)
            {
                Console.WriteLine("WonGame");
            }
        }

        public Vector2 GetPosition()
        {
            return new Vector2(Box.CenterX, Box.CenterY);
        }

        public override void OnBoxChanged()
        {
            CollisionBox = Box;
        }

        public bool CanMove()
        {
            return IsAlive;
        }

        public bool IsTrigger()
        {
            return false;
        }
    }
}