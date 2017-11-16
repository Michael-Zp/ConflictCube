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
        public bool ThrowMode { get; set; } = false;
        public bool UseMode { get; set; } = false;

        /// <summary>
        ///     Create a new player, with a defined size, position and move speed. The collision box of this player equals his rendering box, given with size and position
        /// </summary>
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

        /// <summary>
        ///     Set the center position of the rendering box and call the callback OnBoxChanged()
        /// </summary>
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

        /// <summary>
        ///     Get the Center position of the player box. This will not return the center of the collision box, but the rendering box
        /// </summary>
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


        /// <summary>
        ///     Switches the ThrowMode. If the player is in UseMode, UseMode will be switched off
        /// </summary> 
        public void SwitchThrowMode()
        {
            ThrowMode = !ThrowMode;

            if(ThrowMode)
            {
                UseMode = false;
            }
        }

        /// <summary>
        ///     Switches the UseMode. If the player is in ThrowMode, ThrowMode will be switched off
        /// </summary>
        public void SwitchUseMode()
        {
            UseMode = !UseMode;

            if(UseMode)
            {
                ThrowMode = false;
            }
        }
    }
}