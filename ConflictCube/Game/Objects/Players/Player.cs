using OpenTK;
using System;
using System.Drawing;
using ConflictCube.ResxFiles;
using Engine.Inputs;
using Engine.Time;
using Engine.Components;
using ConflictCube.Debug;
using Engine.View;
using System.ComponentModel.Composition;

namespace ConflictCube.Objects
{
    public abstract class Player : GameObject
    {
#pragma warning disable 0649

        [Import(typeof(ITime))]
        private ITime Time;

#pragma warning restore 0649

        public float Speed { get; protected set; } = 20;
        public bool IsAlive { get; set; } = true;
        public float MaxSprintEnergy = 100;
        public float CurrentSprintEnergy = 100;
        public float UsedSprintEnergyPerSecond = 100;
        public float RegeneratSprintEnergyPerSecond = 20;
        public bool CanMove = true;
        public Player OtherPlayer { private get; set; }

        private IGameManager _GameManager = null;
        private IGameManager GameManager {
            get {
                if (_GameManager == null)
                {
                    _GameManager = (GameManager)GameObject.FindGameObjectByType<GameManager>();
                }
                return _GameManager;
            }
        }
        private ISwitchPlayers _SwitchPlayer = null;
        private ISwitchPlayers SwitchPlayers {
            get {
                if (_SwitchPlayer == null)
                {
                    _SwitchPlayer = (PlayerSwitcher)GameObject.FindGameObjectByType<PlayerSwitcher>();
                }
                return _SwitchPlayer;
            }
        }

        private float UseCooldown = 1.0f;
        private float LastUse { get; set; }

        private float ThrowUseXOffset = 0;
        private float ThrowUseYOffset = 1;
        private float LastUseFieldUpdate = 0;
        private float UseFieldUpdateCooldown = 0.1f;

        private float LastFrameSpeed = 0;
        private AudioPlayer FootstepsSound;

        protected Floor CurrentFloor;
        protected UseField UseField { get; set; }

        protected string Horizontal;
        protected string Vertical;
        protected string Sprint;
        protected string HitBlock;
        protected string SwitchPositionY;
        protected string SwitchPositionXY;
        protected string SwitchPositionX;
        protected int ActiveGamePad = 0;

        protected GameObject AfterglowY;
        protected GameObject AfterglowX;
        protected GameObject AfterglowXY;


        protected abstract void HitSelectedBlock();
        protected abstract bool UseFieldIsOnUsableField();


        /// <summary>
        ///     Create a new player, with a defined size, position and move speed. The collision box of this player equals his rendering box, given with size and position
        /// </summary>
        protected Player(string name, Floor currentFloor, GameObject parent, string playerType, string collisionType, string collisionLayer) : base(name, new Transform(0, 0, .06f, .06f), parent, playerType)
        {
            Program.Container.ComposeParts(this);

            CurrentFloor = currentFloor;
            LastUse = -UseCooldown;

            BoxCollider collider = new BoxCollider(new Transform(0, 0, 1, 1), false, currentFloor.CollisionGroup, collisionType, collisionLayer);

            if (DebugGame.NoClip)
            {
                collider.IsTrigger = true;
            }

            AddComponent(collider);
            
            UseField = new UseField("ThrowUseIndicator", new Transform(), CurrentFloor);
            SetUseFieldWithOffset();


            FootstepsSound = new AudioPlayer(AudioResources.WalkingFast, true);
            AddComponent(FootstepsSound);


            ResetPositionToLastCheckpoint();
        }

        public override void OnUpdate()
        {
            if (!IsAlive)
            {
                return;
            }

            Vector2 moveVector = new Vector2(Input.GetAxis(Horizontal, ActiveGamePad), Input.GetAxis(Vertical, ActiveGamePad));

            if ((Input.OnButtonIsPressed(Sprint, ActiveGamePad) || Input.OnButtonDown(Sprint, ActiveGamePad))
                && CurrentSprintEnergy > UsedSprintEnergyPerSecond * Time.DifTime
                && (moveVector.X != 0 || moveVector.Y != 0))
            {
                moveVector *= (Speed * 2);
                CurrentSprintEnergy -= UsedSprintEnergyPerSecond * Time.DifTime;
            }
            else
            {
                moveVector *= Speed;
                CurrentSprintEnergy += RegeneratSprintEnergyPerSecond * Time.DifTime;
            }

            CurrentSprintEnergy = MathHelper.Clamp(CurrentSprintEnergy, 0, MaxSprintEnergy);

            if (Input.OnButtonDown(HitBlock, ActiveGamePad) && Time.CooldownIsOver(LastUse, UseCooldown))
            {
                LastUse = Time.CurrentTime;
                HitSelectedBlock();
            }


            if (Input.OnButtonDown(SwitchPositionY, ActiveGamePad))
            {
                SwitchPlayers.SwitchYAxis();
            }

            if (Input.OnButtonDown(SwitchPositionXY, ActiveGamePad))
            {
                SwitchPlayers.SwitchXYAxis();
            }

            if (Input.OnButtonDown(SwitchPositionX, ActiveGamePad))
            {
                SwitchPlayers.SwitchXAxis();
            }

            moveVector *= Time.DifTime;

            Move(moveVector);

            //l < 0.03 && LastFS > 0.03 -> MoveSpeed == 0
            if (moveVector.Length < 0.03f)
            {
                if (LastFrameSpeed > 0.03f)
                {
                    FootstepsSound.StopAudio();
                }
            }
            else
            {
                if (LastFrameSpeed < 0.03f)
                {
                    FootstepsSound.PlayAudio();
                }
            }

            LastFrameSpeed = moveVector.Length;

            UpdateUseField();
        }

        public bool LevelTileIsWalkable(Vector2 position)
        {
            Vector2 size = Transform.TransformToGlobal().GetSize(WorldRelation.Global);
            Vector2 gridPos = CurrentFloor.GetGridPosition(new Transform(position.X, position.Y, size.X, size.Y));

            switch (CurrentFloor.FloorTiles[(int)gridPos.Y, (int)gridPos.X].Type)
            {
                case "Wall":
                    return false;
            }

            switch (CurrentFloor.CubeTiles[(int)gridPos.Y, (int)gridPos.X].Type)
            {
                case "OrangeBlock":
                    return false;
                case "BlueBlock":
                    return false;
            }

            return true;
        }
        


        private void UpdateUseField()
        {
            Input.AxesSettings.TryGetValue(Horizontal, out AxisData horizontalAxisData);
            Input.AxesSettings.TryGetValue(Vertical, out AxisData verticalAxisData);

            bool axisIsInUse = Math.Abs(Input.GetAxis(Horizontal, ActiveGamePad)) + Math.Abs(Input.GetAxis(Vertical, ActiveGamePad)) > .4f;

            bool horizontalPositive = Input.OnButtonIsPressed(horizontalAxisData.PositiveKey, ActiveGamePad) || (axisIsInUse && Input.GetAxis(Horizontal, ActiveGamePad) > 0.1f);
            bool horizontalNegative = Input.OnButtonIsPressed(horizontalAxisData.NegativeKey, ActiveGamePad) || (axisIsInUse && Input.GetAxis(Horizontal, ActiveGamePad) < -0.1f);
            bool verticalPositive = Input.OnButtonIsPressed(verticalAxisData.PositiveKey, ActiveGamePad) || (axisIsInUse && Input.GetAxis(Vertical, ActiveGamePad) > 0.1f);
            bool verticalNegative = Input.OnButtonIsPressed(verticalAxisData.NegativeKey, ActiveGamePad) || (axisIsInUse && Input.GetAxis(Vertical, ActiveGamePad) < -0.1f);

            if ((horizontalPositive || horizontalNegative || verticalPositive || verticalNegative) && Time.CooldownIsOver(LastUseFieldUpdate, UseFieldUpdateCooldown))
            {
                LastUseFieldUpdate = Time.CurrentTime;
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

            if (UseFieldIsOnUsableField())
            {
                UseField.Enabled = true;
            }
            else
            {
                UseField.Enabled = false;
            }
        }


        public void Move(Vector2 moveVector)
        {
            if (!CanMove)
            {
                return;
            }

            if (moveVector.Length != 0)
            {
                float targetAngle = 0;

                if (moveVector.X == 0 && moveVector.Y < 0)
                {
                    targetAngle = 180;
                }
                else if (moveVector.X == 0 && moveVector.Y > 0)
                {
                    targetAngle = 0;
                }
                else
                {
                    targetAngle = (float)MathHelper.RadiansToDegrees(Math.Acos(Vector2.Dot(new Vector2(0, 1), moveVector.Normalized())));


                    if (moveVector.X < 0)
                    {
                        targetAngle = 360 - targetAngle;
                    }
                }

                float currentAngle = Transform.GetRotation(WorldRelation.Local);
                float angleDist = targetAngle - currentAngle;


                float angle = currentAngle + angleDist * (10f * Time.DifTime);

                Transform.SetRotation(angle, WorldRelation.Local);

            }

            Transform.MoveRelative(moveVector);
        }

        public override void OnCollision(Collider other)
        {
            if (DebugGame.PlayerPrintCollisionTypes)
            {
                Console.WriteLine(other.Type);
            }

            if (other.Type.Equals("Hole"))
            {
                Die(Name + " fell into a hole");
            }
        }

        public void Die(string reason)
        {
            if (DebugGame.CanDie && IsAlive)
            {
                IsAlive = false;
                GameManager.SetDeathReason(reason);
            }
        }

        public string GetTypeOfFloortileAtPlayerPos()
        {
            Vector2 currentGridPosition = CurrentFloor.GetGridPosition(Transform);
            return CurrentFloor.FloorTiles[(int)currentGridPosition.Y, (int)currentGridPosition.X].Type;
        }


        public abstract void ResetPositionToLastCheckpoint();

        private void SetUseFieldWithOffset(float xOffset = 0, float yOffset = 0)
        {
            Vector2 currentPos = CurrentFloor.GetGridPosition(Transform.TransformToGlobal()) + new Vector2(xOffset, -yOffset);
            Transform throwUseTransform = CurrentFloor.GetBoxAtGridPosition(currentPos);
            UseField.Transform.SetPosition(throwUseTransform.GetPosition(WorldRelation.Global), WorldRelation.Global);
            UseField.Transform.SetSize(throwUseTransform.GetSize(WorldRelation.Global), WorldRelation.Global);

            if (DebugGame.DebugDrawUseField)
            {
                GameView.DrawDebug(UseField.Transform.TransformToGlobal(), Color.Red);
            }

        }

        protected LevelTile GetLevelTileOnCubeLayerOfSelectedField()
        {
            Vector2 useFieldGridPos = CurrentFloor.GetGridPosition(Transform.TransformToGlobal()) + new Vector2(ThrowUseXOffset, -ThrowUseYOffset);

            int indexOfColumn = (int)useFieldGridPos.X;
            int indexOfRow = (int)useFieldGridPos.Y;

            indexOfRow = MathHelper.Clamp(indexOfRow, 0, CurrentFloor.FloorRows - 1);
            indexOfColumn = MathHelper.Clamp(indexOfColumn, 0, CurrentFloor.FloorColumns - 1);


            return CurrentFloor.CubeTiles[indexOfRow, indexOfColumn];
        }
    }
}