using OpenTK;
using System;
using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    public class Player : GameObject
    {
        public float Speed { get; private set; }
        public bool IsAlive { get; private set; }
        public bool ThrowMode { get; set; } = false;
        public bool UseMode { get; set; } = false;
        public float MaxSprintEnergy = 100;
        public float CurrentSprintEnergy = 100;
        public float UsedSprintEnergyPerSecond = 100;
        public float RegeneratSprintEnergyPerSecond = 20;
        
        private Transform _ThrowUseField { get; set; }
        public Transform ThrowUseField {
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
        private Floor CurrentFloor;

        /// <summary>
        ///     Create a new player, with a defined size, position and move speed. The collision box of this player equals his rendering box, given with size and position
        /// </summary>
        public Player(string name, Transform transform, BoxCollider boxCollider, Material material, GameObject parent, Floor currentFloor, float speed, bool isAlive = true) : base(name, transform, parent, GameObjectType.Player)
        {
            Speed = speed;
            IsAlive = isAlive;
            CurrentFloor = currentFloor;
            LastThrowUseFieldUpdate = -ThrowUseFieldUpdateCooldown;

            AddComponent(boxCollider);
            AddComponent(material);
        }

        public override void OnUpdate()
        {
            Vector2 moveVector = new Vector2(Input.GetAxis(InputAxis.Horizontal), Input.GetAxis(InputAxis.Vertical));

            if((Input.OnButtonIsPressed(InputKey.PlayerOneSprint) || Input.OnButtonDown(InputKey.PlayerOneSprint)) && CurrentSprintEnergy > UsedSprintEnergyPerSecond * Time.Time.DifTime)
            {
                moveVector *= (Speed * 2);
                CurrentSprintEnergy -= UsedSprintEnergyPerSecond * Time.Time.DifTime;
            }
            else
            {
                moveVector *= Speed;
                CurrentSprintEnergy += RegeneratSprintEnergyPerSecond * Time.Time.DifTime;
            }

            CurrentSprintEnergy = MathHelper.Clamp(CurrentSprintEnergy, 0, MaxSprintEnergy);

            Console.WriteLine(CurrentSprintEnergy);

            /*if (Input.OnButtonIsPressed(InputKey.PlayerOneMoveUp))
            {
                moveVector.Y += Speed;
            }

            if (Input.OnButtonIsPressed(InputKey.PlayerOneMoveDown))
            {
                moveVector.Y -= Speed;
            }

            if (Input.OnButtonIsPressed(InputKey.PlayerOneMoveRight))
            {
                moveVector.X += Speed;
            }

            if (Input.OnButtonIsPressed(InputKey.PlayerOneMoveLeft))
            {
                moveVector.X -= Speed;
            }*/

            Move(moveVector);
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
                        ThrowUseAddition.X += CurrentFloor.FloorTileSize.X;
                    }
                    else if (moveVector.X < 0)
                    {
                        ThrowUseAddition.X -= CurrentFloor.FloorTileSize.X;
                    }

                    if (moveVector.Y > 0)
                    {
                        ThrowUseAddition.Y += CurrentFloor.FloorTileSize.Y;
                    }
                    else if (moveVector.Y < 0)
                    {
                        ThrowUseAddition.Y -= CurrentFloor.FloorTileSize.Y;
                    }

                    try
                    {
                        ThrowUseField = CurrentFloor.GetBoxAtGridPosition(ThrowUseField.Position + ThrowUseAddition);
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("Hit boundaries with the ThrowUse Field");
                    }
                        
                }
            }
            else
            {
                Console.WriteLine("\n\n\n");
                Transform.MoveRelative(moveVector);
                Console.WriteLine("\n\n\n");
                //Console.WriteLine(Transform.Position);
                //Console.WriteLine(moveVector);
            }
        }

        public override void OnCollision(Collider other)
        {
            Console.WriteLine("Player collided with: " + other.Type.ToString());
            if (other.Type == CollisionType.LeftBoundary || other.Type == CollisionType.RightBoundary || other.Type == CollisionType.TopBoundary || other.Type == CollisionType.Wall)
            {

            }
            else if (other.Type == CollisionType.BottomBoundary || other.Type == CollisionType.Hole)
            {
                IsAlive = false;
            }
            else if (other.Type == CollisionType.Finish)
            {
                Console.WriteLine("WonGame");
                //Environment.Exit(0);
            }
        }
        
        public bool CanMove()
        {
            return IsAlive;
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
            ThrowUseField = CurrentFloor.GetBoxAtGridPosition(new Vector2(Transform.Position.X, Transform.Position.Y));
        }
    }
}