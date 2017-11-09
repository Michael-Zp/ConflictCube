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
            CollidesWith = new HashSet<CollisionType>();

            CollidesWith.Add(CollisionType.LeftBoundary);
            CollidesWith.Add(CollisionType.RightBoundary);
            CollidesWith.Add(CollisionType.TopBoundary);
            CollidesWith.Add(CollisionType.BottomBoundary);
            CollidesWith.Add(CollisionType.Player);
            CollidesWith.Add(CollisionType.Finish);
            CollidesWith.Add(CollisionType.Wall);
            CollidesWith.Add(CollisionType.Hole);
        }

        public void Move(Vector2 moveVector)
        {
            Box.MinX += moveVector.X;
            Box.MinY += moveVector.Y;
        }

        public void SetPosition(Vector2 position)
        {
            Box.CenterX = position.X;
            Box.CenterY = position.Y;
        }

        public void OnCollide(CollisionType type, ICollidable other, Vector2 movementIntoCollision)
        {
            if (type == CollisionType.LeftBoundary || type == CollisionType.RightBoundary || type == CollisionType.TopBoundary || type == CollisionType.Wall )
            {
                Vector2 moveBackVector = new Vector2(0, 0);
                
                //If the player collides with something of these types, put the two boxes as close as possible, but they must not collide (+ 0.000001f)
                if (movementIntoCollision.X != 0)
                {
                    float currentDistance = Math.Abs(CollisionBox.CenterX - other.CollisionBox.CenterX);
                    float minPossibleDistance = CollisionBox.SizeX / 2 + other.CollisionBox.SizeX / 2;
                    float distanceOffset = minPossibleDistance - currentDistance + 0.000001f;

                    if(movementIntoCollision.X > 0)
                    {
                        moveBackVector.X = distanceOffset;
                    }
                    else
                    {
                        moveBackVector.X = -distanceOffset;
                    }
                }


                if (movementIntoCollision.Y != 0)
                {
                    float currentDistance = Math.Abs(CollisionBox.CenterY - other.CollisionBox.CenterY);
                    float minPossibleDistance = CollisionBox.SizeY / 2 + other.CollisionBox.SizeY / 2;
                    float distanceOffset = minPossibleDistance - currentDistance + 0.000001f;

                    if (movementIntoCollision.Y > 0)
                    {
                        moveBackVector.Y = distanceOffset;
                    }
                    else
                    {
                        moveBackVector.Y = -distanceOffset;
                    }
                }


                Move(-moveBackVector);
            }

            else if (type == CollisionType.BottomBoundary || type == CollisionType.Hole)
            {
                //IsAlive = false;
            }

            else if (type == CollisionType.Finish)
            {
                Console.WriteLine("WonGame");
            }
        }

        public Vector2 GetPosition()
        {
            return new Vector2(Box.CenterX, Box.CenterY);
        }
    }
}