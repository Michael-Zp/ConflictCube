using System.Collections.Generic;
using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;
using Zenseless.Geometry;
using System;

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

        public void OnCollide(ICollidable other)
        {
            if (other.CollisionType == CollisionType.LeftBoundary || other.CollisionType == CollisionType.RightBoundary || other.CollisionType == CollisionType.TopBoundary || other.CollisionType == CollisionType.Wall )
            {
                /*
                Vector2 moveBackVector = new Vector2(0, 0);
                //If the player collides with something of these types, put the two boxes as close as possible, but they must not collide (+ 0.000001f)
                if (MoveVectorThisIteration.X != 0)
                {
                    float currentDistance = Math.Abs(CollisionBox.CenterX - other.CollisionBox.CenterX);
                    float minPossibleDistance = CollisionBox.SizeX / 2 + other.CollisionBox.SizeX / 2;
                    float distanceOffset = minPossibleDistance - currentDistance + 0.0001f;

                    if(MoveVectorThisIteration.X > 0)
                    {
                        moveBackVector.X = distanceOffset;
                    }
                    else
                    {
                        moveBackVector.X = -distanceOffset;
                    }
                }


                if (MoveVectorThisIteration.Y != 0)
                {
                    float currentDistance = Math.Abs(CollisionBox.CenterY - other.CollisionBox.CenterY);
                    float minPossibleDistance = CollisionBox.SizeY / 2 + other.CollisionBox.SizeY / 2;
                    float distanceOffset = minPossibleDistance - currentDistance + 0.0001f;

                    if (MoveVectorThisIteration.Y > 0)
                    {
                        moveBackVector.Y = distanceOffset;
                    }
                    else
                    {
                        moveBackVector.Y = -distanceOffset;
                    }
                }*/


                //this.MoveInstantly(-moveBackVector);
                this.MoveInstantly(-this.MoveVectorThisIteration);
            }

            else if (other.CollisionType == CollisionType.BottomBoundary || other.CollisionType == CollisionType.Hole)
            {
                IsAlive = false;
            }

            else if (other.CollisionType == CollisionType.Finish)
            {
                Console.WriteLine("WonGame");
            }


            this.ClearMoveVector();
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