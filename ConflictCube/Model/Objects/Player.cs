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
        
        private Tuple<Vector2, FloorArea> _ThrowUseField { get; set; }
        public Tuple<Vector2, FloorArea> ThrowUseField {
            get {
                if (_ThrowUseField == null)
                {
                    ResetThrowUseField();
                }
                return _ThrowUseField;
            }
            private set {
                _ThrowUseField = value;
            }
        }


        /// <summary>
        ///     Wait time until the ThrowUseField vector can be updated again.
        /// </summary>
        private float ThrowUseFieldUpdateCooldown = 0.1f;
        private float LastThrowUseFieldUpdate { get; set; }

        private Level CurrentLevel { get; set; }

        /// <summary>
        ///     Create a new player, with a defined size, position and move speed. The collision box of this player equals his rendering box, given with size and position
        /// </summary>
        public Player(Vector2 size, Vector2 position, float speed, Level currentLevel, bool isAlive = true) : base(position, size, TileType.Player)
        {
            CollisionBox = Box;
            Speed = speed;
            IsAlive = isAlive;
            InitializeCollision();
            LastThrowUseFieldUpdate = -ThrowUseFieldUpdateCooldown;
            CurrentLevel = currentLevel;
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
        ///     First check if the player can move. If the player is neither in Throw or Use Mode, add Movement to its MoveVectorThisIteration which will be executed in CollisionGroup.MoveThisIteration().
        ///     If the player is in Throw/Use Mode, move the target of the throw or use.
        /// </summary>
        /// <param name="moveVector"></param>
        public void Move(Vector2 moveVector)
        {
            if (!CanMove())
            {
                return;
            }


            if(ThrowMode || UseMode)
            {
                if(Time.Time.CooldownIsOver(LastThrowUseFieldUpdate, ThrowUseFieldUpdateCooldown) && (moveVector.X != 0 || moveVector.Y != 0))
                {
                    LastThrowUseFieldUpdate = Time.Time.CurrentTime;

                    Vector2 ThrowUseAddition = new Vector2(0, 0);

                    if (moveVector.X > 0)
                    {
                        ThrowUseAddition.X += 1;
                    }
                    else if (moveVector.X < 0)
                    {
                        ThrowUseAddition.X -= 1;
                    }

                    if (moveVector.Y > 0)
                    {
                        ThrowUseAddition.Y += 1;
                    }
                    else if (moveVector.Y < 0)
                    {
                        ThrowUseAddition.Y -= 1;
                    }

                    try
                    {
                        ThrowUseField = CurrentLevel.GetPositionInLevelWithOffset(ThrowUseField, ThrowUseAddition);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Hit boundaries with the ThrowUse Field");
                    }
                        
                }
            }
            else
            {
                MoveVectorThisIteration += moveVector;
            }
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
                Environment.Exit(0);
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
        ///     If the Player changed from normal mode into throw/use mode the position of the resulting throw will be reset to 0, 0 offset from the player
        ///     Updates the ThrowUseField if necessary.
        /// </summary> 
        public void SwitchThrowMode()
        {
            if (!ThrowMode && !UseMode)
            {
                ResetThrowUseField();
            }

            ThrowMode = !ThrowMode;

            if (ThrowMode)
            {
                UseMode = false;
            }
        }

        /// <summary>
        ///     Switches the UseMode. If the player is in ThrowMode, ThrowMode will be switched off
        ///     If the Player changed from normal mode into throw/use mode the position of the resulting throw will be reset to 0, 0 offset from the player
        ///     Updates the ThrowUseField if necessary.
        /// </summary>
        public void SwitchUseMode()
        {
            if (!ThrowMode && !UseMode)
            {
                ResetThrowUseField();
            }

            UseMode = !UseMode;

            if (UseMode)
            {
                ThrowMode = false;
            }
        }

        private void ResetThrowUseField()
        {
            _ThrowUseField = CurrentLevel.GetGridPosition(new Vector2(Box.CenterX, Box.CenterY));
        }
    }
}