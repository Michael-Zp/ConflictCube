using OpenTK;
using System;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;
using Zenseless.OpenGL;

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
        protected static Material UseMaterialForeground;
        protected static Material UseMaterialBackground;
        protected float ThrowUseXOffset = 0;
        protected float ThrowUseYOffset = 1;
        protected float LastUseFieldUpdate = 0;
        protected float UseFieldUpdateCooldown = 0.1f;

        protected InputAxis Horizontal;
        protected InputAxis Vertical;
        protected InputKey Sprint;
        protected InputKey HitBlock;
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

            boxCollider.IgnoreCollisionsWith.Add(CollisionType.PlayerFire);
            boxCollider.IgnoreCollisionsWith.Add(CollisionType.PlayerIce);
            AddComponent(boxCollider);
            AddComponent(material);

            if (!MaterialsAreInitialized)
            {
                MaterialsAreInitialized = true;
                UseMaterialForeground   = new Material(TextureLoader.FromBitmap(TexturResource.UseFieldIndicator), new Zenseless.Geometry.Box2D(0, 0, 1, 1), System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
                UseMaterialBackground   = new Material(null, null, System.Drawing.Color.FromArgb(32, 0, 255, 64));
            }

            UseField = new ColoredBox("ThrowUseIndicator", new Transform(), UseMaterialForeground, this);
            UseField.AddComponent(UseMaterialBackground);
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
            Input.AxesSettings.TryGetValue(Horizontal, out AxisData horizontalAxisData);
            Input.AxesSettings.TryGetValue(Vertical,   out AxisData verticalAxisData);

            bool horizontalPositive = Input.OnButtonIsPressed(horizontalAxisData.PositiveKey, ActiveGamePad);
            bool horizontalNegative = Input.OnButtonIsPressed(horizontalAxisData.NegativeKey, ActiveGamePad);
            bool verticalPositive = Input.OnButtonIsPressed(verticalAxisData.PositiveKey, ActiveGamePad);
            bool verticalNegative = Input.OnButtonIsPressed(verticalAxisData.NegativeKey, ActiveGamePad);

            if((horizontalPositive || horizontalNegative || verticalPositive || verticalNegative) && Time.Time.CooldownIsOver(LastUseFieldUpdate, UseFieldUpdateCooldown))
            {
                LastUseFieldUpdate = Time.Time.CurrentTime;
                if (horizontalPositive && !horizontalNegative)
                {
                    ThrowUseXOffset = 1;
                }
                else if (!horizontalPositive && horizontalNegative)
                {
                    ThrowUseXOffset = -1;
                }
                else
                {
                    ThrowUseXOffset = 0;
                }

                if (verticalPositive && !verticalNegative)
                {
                    ThrowUseYOffset = 1;
                }
                else if (!verticalPositive && verticalNegative)
                {
                    ThrowUseYOffset = -1;
                }
                else
                {
                    ThrowUseYOffset = 0;
                }
            }
            
            try
            {
                Vector2 currentPos = CurrentFloor.GetGridPosition(Transform.TransformToGlobal());

                if (currentPos.X + ThrowUseXOffset > CurrentFloor.FloorColumns - 1)
                {
                    ThrowUseXOffset = CurrentFloor.FloorColumns - 1;
                }
                else if (currentPos.X + ThrowUseXOffset < 0)
                {
                    ThrowUseXOffset = 0;
                }

                if (currentPos.Y + ThrowUseYOffset > CurrentFloor.FloorRows - 1)
                {
                    ThrowUseYOffset = CurrentFloor.FloorRows - 1;
                }
                else if (currentPos.Y + ThrowUseYOffset < 0)
                {
                    ThrowUseYOffset = 0;
                }

                SetUseFieldWithOffset(ThrowUseXOffset, ThrowUseYOffset);
            }
            catch (Exception)
            {
                SetUseFieldWithOffset();

                Console.WriteLine("Hit boundaries with the ThrowUse Field");
            }
        }
        
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