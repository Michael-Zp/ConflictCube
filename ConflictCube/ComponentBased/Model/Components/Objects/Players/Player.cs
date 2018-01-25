using OpenTK;
using System;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;
using System.Drawing;
using ConflictCube.ResxFiles;
using ConflictCube.ComponentBased.Model.Components.Objects.Players;
using ConflictCube.ComponentBased.Model.Components.Colliders;
using ConflictCube.ComponentBased.Model.Components.Objects;

namespace ConflictCube.ComponentBased
{
    public abstract class Player : GameObject
    {
        public float Speed { get; protected set; } = 20;
        public bool IsAlive { get; set; } = true;
        public float MaxSprintEnergy = 100;
        public float CurrentSprintEnergy = 100;
        public float UsedSprintEnergyPerSecond = 100;
        public float RegeneratSprintEnergyPerSecond = 20;
        public bool CanMove = true;
        public Player OtherPlayer { private get; set; }

        private IGameManager _GameManager;
        private IGameManager GameManager {
            get {
                if(_GameManager == null)
                {
                    _GameManager = (GameManager)GameObject.FindGameObjectByType<GameManager>();
                }
                return _GameManager;
            }
        }
        private ISwitchPlayers _SwitchPlayer;
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
        
        protected Floor CurrentFloor;
        protected UseField UseField { get; set; }

        protected InputAxis Horizontal;
        protected InputAxis Vertical;
        protected InputKey Sprint;
        protected InputKey HitBlock;
        protected InputKey SwitchPositionY;
        protected InputKey SwitchPositionXY;
        protected InputKey SwitchPositionX;
        protected int ActiveGamePad = 0;

        protected Material AfterglowMaterialY;
        protected Material AfterglowMaterialX;
        protected Material AfterglowMaterialXY;
        private static float AfterglowLifetime = .5f;

        protected GameObject AfterglowY;
        protected GameObject AfterglowX;
        protected GameObject AfterglowXY;


        protected abstract void HitSelectedBlock();
        protected abstract bool UseFieldIsOnUsableField();


        /// <summary>
        ///     Create a new player, with a defined size, position and move speed. The collision box of this player equals his rendering box, given with size and position
        /// </summary>
        protected Player(string name, Floor currentFloor, GameObject parent, GameObjectType playerType, CollisionType collisionType, CollisionLayer collisionLayer) : base(name, new Transform(0, 0, .06f, .06f), parent, playerType)
        {
            CurrentFloor = currentFloor;
            LastUse = -UseCooldown;

            BoxCollider collider = new BoxCollider(new Transform(0, 0, 1, 1), false, currentFloor.CollisionGroup, collisionType, collisionLayer);

            if (DebugGame.NoClip)
            {
                collider.IsTrigger = true;
            }

            AddComponent(collider);

            
            AfterglowMaterialY = new Material(Color.White, ShaderResources.Afterglow);
            AfterglowMaterialY.AddShaderParameter("startTime", -AfterglowLifetime * 2);
            AfterglowMaterialY.AddShaderParameter("direction", 1f);
            AfterglowMaterialY.AddShaderParameter("lifetime", AfterglowLifetime);
            AfterglowMaterialY.AddShaderParameter("desiredColor", new Vector3(Color.Pink.R, Color.Pink.G, Color.Pink.B));

            AfterglowMaterialX = new Material(Color.White, ShaderResources.Afterglow);
            AfterglowMaterialX.AddShaderParameter("startTime", -AfterglowLifetime * 2);
            AfterglowMaterialX.AddShaderParameter("direction", 1f);
            AfterglowMaterialX.AddShaderParameter("lifetime", AfterglowLifetime);
            AfterglowMaterialX.AddShaderParameter("desiredColor", new Vector3(Color.Pink.R, Color.Pink.G, Color.Pink.B));

            AfterglowMaterialXY = new Material(Color.White, ShaderResources.Afterglow);
            AfterglowMaterialXY.AddShaderParameter("startTime", -AfterglowLifetime * 2);
            AfterglowMaterialXY.AddShaderParameter("direction", 1f);
            AfterglowMaterialXY.AddShaderParameter("lifetime", AfterglowLifetime);
            AfterglowMaterialXY.AddShaderParameter("desiredColor", new Vector3(Color.Pink.R, Color.Pink.G, Color.Pink.B));

            
            AfterglowY = new GameObject("Afterglow Y", new Transform(), CurrentFloor);
            AfterglowY.AddComponent(AfterglowMaterialY);

            AfterglowX = new GameObject("Afterglow X", new Transform(), CurrentFloor);
            AfterglowX.AddComponent(AfterglowMaterialX);

            AfterglowXY = new GameObject("Afterglow XY", new Transform(), CurrentFloor);
            AfterglowXY.AddComponent(AfterglowMaterialXY);
            

            UseField = new UseField("ThrowUseIndicator", new Transform(), CurrentFloor);
            SetUseFieldWithOffset();

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
            

            if (Input.OnButtonDown(SwitchPositionY, ActiveGamePad))
            {
                SwitchPlayers.SwitchYAxis();

                //Afterglow

                ShowYAfterglow();
                OtherPlayer.ShowYAfterglow();
            }
            
            if (Input.OnButtonDown(SwitchPositionXY, ActiveGamePad))
            {
                SwitchPlayers.SwitchXYAxis();

                //Afterglow

                ShowXYAfterglow();
                OtherPlayer.ShowXYAfterglow();
            }
            
            if (Input.OnButtonDown(SwitchPositionX, ActiveGamePad))
            {
                SwitchPlayers.SwitchXAxis();
                
                //Afterglow

                ShowXAfterglow();
                OtherPlayer.ShowXAfterglow();
            }

            Move(moveVector * Time.Time.DifTime);
            UpdateUseField();
        }

        public bool LevelTileIsWalkable(Vector2 position)
        {
            Vector2 size = Transform.TransformToGlobal().GetSize(WorldRelation.Global);
            Vector2 gridPos = CurrentFloor.GetGridPosition(new Transform(position.X, position.Y, size.X, size.Y));

            switch (CurrentFloor.FloorTiles[(int)gridPos.Y, (int)gridPos.X].Type)
            {
                case GameObjectType.Wall:
                    return false;
            }

            switch(CurrentFloor.CubeTiles[(int)gridPos.Y, (int)gridPos.X].Type)
            {
                case GameObjectType.OrangeBlock:
                    return false;
                case GameObjectType.BlueBlock:
                    return false;
            }

            return true;
        }

        private void ShowYAfterglow()
        {
            Vector2 thisPosition = Transform.GetPosition(WorldRelation.Global);
            Vector2 otherPosition = OtherPlayer.Transform.GetPosition(WorldRelation.Global);

            AfterglowMaterialY.AddShaderParameter("startTime", Time.Time.CurrentTime);

            if (thisPosition.Y >= otherPosition.Y)
            {
                AfterglowMaterialY.AddShaderParameter("direction", -1f);

                AfterglowY.Transform.SetPosition(new Vector2(thisPosition.X, otherPosition.Y + (thisPosition.Y - otherPosition.Y) / 2), WorldRelation.Global);

                float xSize = MathHelper.Clamp((Transform.GetMinY(WorldRelation.Global) - OtherPlayer.Transform.GetMaxY(WorldRelation.Global)) / 2, 0, float.MaxValue);
                AfterglowY.Transform.SetSize(new Vector2(xSize, Transform.GetSize(WorldRelation.Global).X), WorldRelation.Global);
                AfterglowY.Transform.SetRotation(90, WorldRelation.Global);
            }
            else
            {
                AfterglowMaterialY.AddShaderParameter("direction", 1f);

                AfterglowY.Transform.SetPosition(new Vector2(thisPosition.X, thisPosition.Y + (otherPosition.Y - thisPosition.Y) / 2), WorldRelation.Global);

                float xSize = MathHelper.Clamp((OtherPlayer.Transform.GetMinY(WorldRelation.Global) - Transform.GetMaxY(WorldRelation.Global)) / 2, 0, float.MaxValue);
                AfterglowY.Transform.SetSize(new Vector2(xSize, Transform.GetSize(WorldRelation.Global).X), WorldRelation.Global);
                AfterglowY.Transform.SetRotation(90, WorldRelation.Global);
            }
        }


        private void ShowXAfterglow()
        {
            Vector2 thisPosition = Transform.GetPosition(WorldRelation.Global);
            Vector2 otherPosition = OtherPlayer.Transform.GetPosition(WorldRelation.Global);

            AfterglowMaterialY.AddShaderParameter("startTime", Time.Time.CurrentTime);

            if (thisPosition.X >= otherPosition.X)
            {
                AfterglowMaterialY.AddShaderParameter("direction", 1f);

                AfterglowY.Transform.SetPosition(new Vector2(otherPosition.X + (thisPosition.X - otherPosition.X) / 2, thisPosition.Y), WorldRelation.Global);

                float xSize = MathHelper.Clamp((Transform.GetMinX(WorldRelation.Global) - OtherPlayer.Transform.GetMaxX(WorldRelation.Global)) / 2, 0, float.MaxValue);
                AfterglowY.Transform.SetSize(new Vector2(xSize, Transform.GetSize(WorldRelation.Global).X), WorldRelation.Global);
                AfterglowY.Transform.SetRotation(0, WorldRelation.Global);
            }
            else
            {
                AfterglowMaterialY.AddShaderParameter("direction", -1f);

                AfterglowY.Transform.SetPosition(new Vector2(thisPosition.X + (otherPosition.X - thisPosition.X) / 2, thisPosition.Y), WorldRelation.Global);

                float xSize = MathHelper.Clamp((OtherPlayer.Transform.GetMinX(WorldRelation.Global) - Transform.GetMaxX(WorldRelation.Global)) / 2, 0, float.MaxValue);
                AfterglowY.Transform.SetSize(new Vector2(xSize, Transform.GetSize(WorldRelation.Global).X), WorldRelation.Global);
                AfterglowY.Transform.SetRotation(0, WorldRelation.Global);
            }
        }


        private void ShowXYAfterglow()
        {
            Vector2 thisPosition = Transform.GetPosition(WorldRelation.Global);
            Vector2 otherPosition = OtherPlayer.Transform.GetPosition(WorldRelation.Global);

            AfterglowMaterialY.AddShaderParameter("startTime", Time.Time.CurrentTime);
            
            AfterglowMaterialY.AddShaderParameter("direction", 1f);

            AfterglowY.Transform.SetPosition(new Vector2(otherPosition.X + 3 * (thisPosition.X - otherPosition.X) / 4, otherPosition.Y + 3 * (thisPosition.Y - otherPosition.Y) / 4), WorldRelation.Global);

            Components.Rectangle thisRect = Transform.GetGlobalRotatedRectangle();
            Components.Rectangle otherRect = OtherPlayer.Transform.GetGlobalRotatedRectangle();
            float xSize = System.Numerics.Vector2.Distance(new System.Numerics.Vector2(thisRect.BottomLeft.X, thisRect.BottomLeft.Y), new System.Numerics.Vector2(otherRect.BottomLeft.X, otherRect.BottomLeft.Y));
            AfterglowY.Transform.SetSize(new Vector2(xSize / 4, Transform.GetSize(WorldRelation.Global).X), WorldRelation.Global);

            float yDif = Transform.GetMaxY(WorldRelation.Global) - OtherPlayer.Transform.GetMaxY(WorldRelation.Global);
            float xDif = Transform.GetMaxX(WorldRelation.Global) - OtherPlayer.Transform.GetMaxX(WorldRelation.Global);
            AfterglowY.Transform.SetRotation(-MathHelper.RadiansToDegrees((float)Math.Atan2(yDif, xDif)), WorldRelation.Global);
        }



        private void UpdateUseField()
        {
            Input.AxesSettings.TryGetValue(Horizontal, out AxisData horizontalAxisData);
            Input.AxesSettings.TryGetValue(Vertical,   out AxisData verticalAxisData);

            bool axisIsInUse = Math.Abs(Input.GetAxis(Horizontal, ActiveGamePad)) + Math.Abs(Input.GetAxis(Vertical, ActiveGamePad)) > .4f;

            bool horizontalPositive = Input.OnButtonIsPressed(horizontalAxisData.PositiveKey, ActiveGamePad) || (axisIsInUse && Input.GetAxis(Horizontal, ActiveGamePad) > 0.1f);
            bool horizontalNegative = Input.OnButtonIsPressed(horizontalAxisData.NegativeKey, ActiveGamePad) || (axisIsInUse && Input.GetAxis(Horizontal, ActiveGamePad) < -0.1f);
            bool verticalPositive = Input.OnButtonIsPressed(verticalAxisData.PositiveKey, ActiveGamePad) || (axisIsInUse && Input.GetAxis(Vertical, ActiveGamePad) > 0.1f);
            bool verticalNegative = Input.OnButtonIsPressed(verticalAxisData.NegativeKey, ActiveGamePad) || (axisIsInUse && Input.GetAxis(Vertical, ActiveGamePad) < -0.1f);

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

            if(UseFieldIsOnUsableField())
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

            if(moveVector.Length != 0)
            {
                float targetAngle = 0;

                if (moveVector.X == 0 && moveVector.Y < 0)
                {
                    targetAngle = 180;
                }
                else if(moveVector.X == 0 && moveVector.Y > 0)
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


                float angle = currentAngle + angleDist * (10f * Time.Time.DifTime);

                Transform.SetRotation(angle, WorldRelation.Local);

            }

            Transform.MoveRelative(moveVector);
        }

        public override void OnCollision(Collider other)
        {
            if(DebugGame.PlayerPrintCollisionTypes)
            {
                Console.WriteLine(other.Type);
            }
            
            if (other.Type == CollisionType.Hole)
            {
                Die(Name + " fell into a hole");
            }
        }

        public void Die(string reason)
        {
            if(DebugGame.CanDie && IsAlive)
            {
                IsAlive = false;
                GameManager.SetDeathReason(reason);
            }
        }

        public GameObjectType GetTypeOfFloortileAtPlayerPos()
        {
            Vector2 currentGridPosition = CurrentFloor.GetGridPosition(Transform);
            return CurrentFloor.FloorTiles[(int)currentGridPosition.Y, (int)currentGridPosition.X].Type;
        }

        
        public void ResetPositionToLastCheckpoint()
        {
            Transform.SetPosition(CurrentFloor.FindStartPosition().GetPosition(WorldRelation.Global), WorldRelation.Global);
            Transform.SetRotation(0, WorldRelation.Global);
        }

        private void SetUseFieldWithOffset(float xOffset = 0, float yOffset = 0)
        {
            Vector2 currentPos = CurrentFloor.GetGridPosition(Transform.TransformToGlobal()) + new Vector2(xOffset, -yOffset);
            Transform throwUseTransform = CurrentFloor.GetBoxAtGridPosition(currentPos);
            UseField.Transform.SetPosition(throwUseTransform.GetPosition(WorldRelation.Global), WorldRelation.Global);
            UseField.Transform.SetSize(throwUseTransform.GetSize(WorldRelation.Global), WorldRelation.Global);
            
            if(DebugGame.DebugDrawUseField)
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