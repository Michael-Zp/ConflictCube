using OpenTK;
using System;
using ConflictCube.ComponentBased.Components;
using System.Collections.Generic;
using ConflictCube.ComponentBased.Model.Components.Objects;
using ConflictCube.ComponentBased.Components.Objects.Tiles;

namespace ConflictCube.ComponentBased
{
    public abstract class Player : GameObject
    {
        public float Speed { get; protected set; }
        public bool IsAlive { get; protected set; }
        public float MaxSprintEnergy = 100;
        public float CurrentSprintEnergy = 100;
        public float UsedSprintEnergyPerSecond = 100;
        public float RegeneratSprintEnergyPerSecond = 20;

        protected GameObject UseField { get; set; }
        protected float UseCooldown = 1.0f;
        protected float LastUse { get; set; }
        protected Floor CurrentFloor;

        protected static bool MaterialsAreInitialized = false;
        protected static Material UseMaterial;
        protected float ThrowUseXOffset = 0;
        protected float ThrowUseYOffset = 0;

        protected InputAxis Horizontal;
        protected InputAxis Vertical;
        protected InputKey ThrowUseUp;
        protected InputKey ThrowUseDown;
        protected InputKey ThrowUseLeft;
        protected InputKey ThrowUseRight;
        protected InputKey Sprint;
        protected InputKey HitBlock;
        protected InputKey InventoryUp;
        protected InputKey InventoryDown;
        protected int ActiveGamePad = 0;



        /// <summary>
        ///     Create a new player, with a defined size, position and move speed. The collision box of this player equals his rendering box, given with size and position
        /// </summary>
        public Player(string name, Transform transform, BoxCollider boxCollider, Material material, GameObject parent, Floor currentFloor, float speed, GameObjectType playerType, bool isAlive = true) : base(name, transform, parent, playerType)
        {
            Speed = speed;
            IsAlive = isAlive;
            CurrentFloor = currentFloor;
            LastUse = -UseCooldown;

            if(DebugGame.NoClip)
            {
                boxCollider.IsTrigger = true;
            }


            AddComponent(boxCollider);
            AddComponent(material);

            if (!MaterialsAreInitialized)
            {
                MaterialsAreInitialized = true;
                UseMaterial             = new Material(null, null, System.Drawing.Color.FromArgb(128, 0, 255, 0));
            }

            UseField = new ColoredBox("ThrowUseIndicator", new Transform(), UseMaterial, this);
            CurrentFloor.AddChild(UseField);
            SetUseFieldWithOffset();
        }

        public override void OnUpdate()
        {
            Vector2 moveVector = new Vector2(Input.GetAxis(Horizontal, ActiveGamePad), Input.GetAxis(Vertical, ActiveGamePad));

            if ((Input.OnButtonIsPressed(Sprint, ActiveGamePad) || Input.OnButtonDown(Sprint, ActiveGamePad))
                && CurrentSprintEnergy > UsedSprintEnergyPerSecond * Time.Time.DifTime
                && (moveVector.X != 0 || moveVector.Y != 0))
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
            
            if (Input.OnButtonDown(HitBlock, ActiveGamePad) && Time.Time.CooldownIsOver(LastUse, UseCooldown))
            {
                LastUse = Time.Time.CurrentTime;
                HitSelectedBlock();
            }
            
            Move(moveVector);
            UpdateUseField();
        }

        protected abstract void HitSelectedBlock();
        

        private void UpdateUseField()
        {
            if (Input.OnButtonDown(ThrowUseUp, ActiveGamePad))
            {
                ThrowUseYOffset += 1;
            }
            if (Input.OnButtonDown(ThrowUseDown, ActiveGamePad))
            {
                ThrowUseYOffset -= 1;
            }
            if (Input.OnButtonDown(ThrowUseLeft, ActiveGamePad))
            {
                ThrowUseXOffset -= 1;
            }
            if (Input.OnButtonDown(ThrowUseRight, ActiveGamePad))
            {
                ThrowUseXOffset += 1;
            }

            ThrowUseXOffset = MathHelper.Clamp(ThrowUseXOffset, -1, 1);
            ThrowUseYOffset = MathHelper.Clamp(ThrowUseYOffset, -1, 1);

            try
            {
                SetUseFieldWithOffset(ThrowUseXOffset, ThrowUseYOffset);
            }
            catch (Exception)
            {
                Vector2 currentPos = CurrentFloor.GetGridPosition(Transform.TransformToGlobal()) + new Vector2(ThrowUseXOffset, ThrowUseYOffset);

                if (currentPos.X > CurrentFloor.FloorColumns - 1)
                {
                    ThrowUseXOffset -= 1;
                }
                else if (currentPos.X < 0)
                {
                    ThrowUseXOffset += 1;
                }

                if (currentPos.Y > CurrentFloor.FloorRows - 1)
                {
                    ThrowUseYOffset -= 1;
                }
                else if (currentPos.Y < 0)
                {
                    ThrowUseYOffset += 1;
                }

                Console.WriteLine("Hit boundaries with the ThrowUse Field");
            }
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

            Transform.MoveRelative(moveVector);
        }

        public override void OnCollision(Collider other)
        {
            if(DebugGame.PlayerPrintCollisionTypes)
            {
                Console.WriteLine(other.Type);
            }


            if (other.Type == CollisionType.LeftBoundary ||
                other.Type == CollisionType.RightBoundary ||
                other.Type == CollisionType.TopBoundary ||
                other.Type == CollisionType.BottomBoundary ||
                other.Type == CollisionType.Wall)
            {

            }
            else if (other.Type == CollisionType.Hole)
            {
                Die();
            }
            else if (other.Type == CollisionType.Finish)
            {
                Console.WriteLine("WonGame");
                //Environment.Exit(0);
            }
        }

        public void Die()
        {
            if(DebugGame.CanDie)
            {
                IsAlive = false;
            }
        }

        public bool CanMove()
        {
            return IsAlive;
        }

        private void SetUseFieldWithOffset(float xOffset = 0, float yOffset = 0)
        {
            Vector2 currentPos = CurrentFloor.GetGridPosition(Transform.TransformToGlobal()) + new Vector2(xOffset, yOffset);
            Transform throwUseTransform = CurrentFloor.GetBoxAtGridPosition(currentPos);
            UseField.Transform.SetPosition(throwUseTransform.GetPosition(WorldRelation.Global), WorldRelation.Global);
            UseField.Transform.SetSize(throwUseTransform.GetSize(WorldRelation.Global), WorldRelation.Global);
            
            if(DebugGame.DebugDrawUseField)
            {
                GameView.DrawDebug(UseField.Transform.TransformToGlobal(), System.Drawing.Color.Red);
            }

        }

        protected FloorTile GetFloorTileOfUseField()
        {
            Vector2 useFieldGridPos = CurrentFloor.GetGridPosition(Transform.TransformToGlobal()) + new Vector2(ThrowUseXOffset, ThrowUseYOffset);

            int indexOfColumn = (int)useFieldGridPos.X;
            int indexOfRow = CurrentFloor.FloorRows - 1 - (int)useFieldGridPos.Y;

            return CurrentFloor.FloorTiles[indexOfRow, indexOfColumn];
        }
    }
}