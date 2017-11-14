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

                Vector2 onlyXMovement = new Vector2(MoveVectorThisIteration.X, 0f);
                Vector2 onlyYMovement = new Vector2(0f, MoveVectorThisIteration.Y);


                this.MoveInstantly(-MoveVectorThisIteration);


                this.MoveInstantly(onlyXMovement);

                if(other.CollisionBox.Intersects(CollisionBox))
                {
                    float yDistance = Math.Abs(Math.Abs(other.CollisionBox.CenterY - CollisionBox.CenterY) - other.CollisionBox.SizeY / 2 - CollisionBox.SizeY / 2);

                    if(yDistance > 0.001f)
                    {
                        float xDif = 0;
                        if (onlyXMovement.X > 0)
                        {
                            xDif = ((other.CollisionBox.MinX - CollisionBox.SizeX) + 0.0000001f) - CollisionBox.MinX;
                        }
                        else if (onlyXMovement.X < 0)
                        {
                            xDif = (other.CollisionBox.MaxX - 0.0000001f) - CollisionBox.MinX;
                        }
                        this.MoveInstantly(new Vector2(xDif, 0f));
                    }
                }

                this.MoveInstantly(onlyYMovement);

                if(other.CollisionBox.Intersects(CollisionBox))
                {
                    float xDistance = Math.Abs(Math.Abs(other.CollisionBox.CenterX - CollisionBox.CenterX) - other.CollisionBox.SizeX / 2 - CollisionBox.SizeX / 2);

                    if (xDistance > 0.001f)
                    {
                        float yDif = 0;
                        if (onlyYMovement.Y > 0)
                        {
                            yDif = ((other.CollisionBox.MinY - CollisionBox.SizeY) + 0.0000001f) - CollisionBox.MinY;
                        }
                        else if (onlyYMovement.Y < 0)
                        {
                            yDif = (other.CollisionBox.MaxY - 0.0000001f) - CollisionBox.MinY;
                        }
                        this.MoveInstantly(new Vector2(0f, yDif));
                    }
                }
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
    }
}