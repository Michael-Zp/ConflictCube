﻿using OpenTK;
using System;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;
using Zenseless.OpenGL;
using System.Drawing;

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
        public Player OtherPlayer;

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
        protected InputKey SwitchPositionY;
        protected InputKey SwitchPositionXY;
        protected InputKey SwitchPositionX;
        protected int ActiveGamePad = 0;

        protected Material AfterglowMaterialY;
        protected Material AfterglowMaterialX;
        protected Material AfterglowMaterialXY;
        protected static float AfterglowLifetime = .25f;

        protected GameObject AfterglowY;
        protected GameObject AfterglowX;
        protected GameObject AfterglowXY;


        /// <summary>
        ///     Create a new player, with a defined size, position and move speed. The collision box of this player equals his rendering box, given with size and position
        /// </summary>
        public Player(string name, Transform transform, BoxCollider boxCollider, Material material, GameObject parent, Floor currentFloor, float speed, GameObjectType playerType, Player otherPlayer, bool isAlive = true) : base(name, transform, parent, playerType)
        {
            Speed = speed;
            IsAlive = isAlive;
            CurrentFloor = currentFloor;
            LastUse = -UseCooldown;
            OtherPlayer = otherPlayer;

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
                UseMaterialForeground   = new Material(Color.FromArgb(255, Color.White), TextureLoader.FromBitmap(TexturResource.UseFieldIndicator), new Zenseless.Geometry.Box2D(0, 0, 1, 1));
                UseMaterialBackground   = new Material(Color.FromArgb(32, 0, 255, 64), null, null);
            }


            
            AfterglowMaterialY = new Material(Color.White, ShaderResources.Afterglow);
            AfterglowMaterialY.AddShaderParameter("startTime", -AfterglowLifetime);
            AfterglowMaterialY.AddShaderParameter("direction", 1f);
            AfterglowMaterialY.AddShaderParameter("lifetime", AfterglowLifetime);
            AfterglowMaterialY.AddShaderParameter("desiredColor", new Vector3(Color.Pink.R, Color.Pink.G, Color.Pink.B));

            AfterglowMaterialX = new Material(Color.White, ShaderResources.Afterglow);
            AfterglowMaterialX.AddShaderParameter("startTime", -AfterglowLifetime);
            AfterglowMaterialX.AddShaderParameter("direction", 1f);
            AfterglowMaterialX.AddShaderParameter("lifetime", AfterglowLifetime);
            AfterglowMaterialX.AddShaderParameter("desiredColor", new Vector3(Color.Pink.R, Color.Pink.G, Color.Pink.B));

            AfterglowMaterialXY = new Material(Color.White, ShaderResources.Afterglow);
            AfterglowMaterialXY.AddShaderParameter("startTime", -AfterglowLifetime);
            AfterglowMaterialXY.AddShaderParameter("direction", 1f);
            AfterglowMaterialXY.AddShaderParameter("lifetime", AfterglowLifetime);
            AfterglowMaterialXY.AddShaderParameter("desiredColor", new Vector3(Color.Pink.R, Color.Pink.G, Color.Pink.B));

            
            AfterglowY = new GameObject("Afterglow Y", new Transform());
            AfterglowY.AddComponent(AfterglowMaterialY);
            AddChild(AfterglowY);

            AfterglowX = new GameObject("Afterglow X", new Transform());
            AfterglowX.AddComponent(AfterglowMaterialX);
            AddChild(AfterglowX);

            AfterglowXY = new GameObject("Afterglow XY", new Transform());
            AfterglowXY.AddComponent(AfterglowMaterialXY);
            AddChild(AfterglowXY);
            

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

            if (Input.OnButtonDown(SwitchPositionY, ActiveGamePad))
            {
                Vector2 thisPosition = Transform.GetPosition(WorldRelation.Global);
                Vector2 otherPosition = OtherPlayer.Transform.GetPosition(WorldRelation.Global);

                float temp = thisPosition.Y;
                thisPosition.Y = otherPosition.Y;
                otherPosition.Y = temp;
                
                Transform.SetPosition(thisPosition, WorldRelation.Global);
                OtherPlayer.Transform.SetPosition(otherPosition, WorldRelation.Global);


                //Afterglow

                ShowYAfterglow();
                OtherPlayer.ShowYAfterglow();

            }
            
            if (Input.OnButtonDown(SwitchPositionXY, ActiveGamePad))
            {
                Vector2 thisPosition = Transform.GetPosition(WorldRelation.Global);
                Vector2 otherPosition = OtherPlayer.Transform.GetPosition(WorldRelation.Global);
                
                Transform.SetPosition(otherPosition, WorldRelation.Global);
                OtherPlayer.Transform.SetPosition(thisPosition, WorldRelation.Global);

                //Afterglow

                ShowXYAfterglow();
                OtherPlayer.ShowXYAfterglow();
            }
            
            if (Input.OnButtonDown(SwitchPositionX, ActiveGamePad))
            {
                Vector2 thisPosition = Transform.GetPosition(WorldRelation.Global);
                Vector2 otherPosition = OtherPlayer.Transform.GetPosition(WorldRelation.Global);

                float temp = thisPosition.X;
                thisPosition.X = otherPosition.X;
                otherPosition.X = temp;

                Transform.SetPosition(thisPosition, WorldRelation.Global);
                OtherPlayer.Transform.SetPosition(otherPosition, WorldRelation.Global);

                //Afterglow

                ShowXAfterglow();
                OtherPlayer.ShowXAfterglow();
            }

            Move(moveVector);
            UpdateUseField();
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
                AfterglowY.Transform.SetRotation(90);
            }
            else
            {
                AfterglowMaterialY.AddShaderParameter("direction", 1f);

                AfterglowY.Transform.SetPosition(new Vector2(thisPosition.X, thisPosition.Y + (otherPosition.Y - thisPosition.Y) / 2), WorldRelation.Global);

                float xSize = MathHelper.Clamp((OtherPlayer.Transform.GetMinY(WorldRelation.Global) - Transform.GetMaxY(WorldRelation.Global)) / 2, 0, float.MaxValue);
                AfterglowY.Transform.SetSize(new Vector2(xSize, Transform.GetSize(WorldRelation.Global).X), WorldRelation.Global);
                AfterglowY.Transform.SetRotation(90);
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
                AfterglowY.Transform.SetRotation(0);
            }
            else
            {
                AfterglowMaterialY.AddShaderParameter("direction", -1f);

                AfterglowY.Transform.SetPosition(new Vector2(thisPosition.X + (otherPosition.X - thisPosition.X) / 2, thisPosition.Y), WorldRelation.Global);

                float xSize = MathHelper.Clamp((OtherPlayer.Transform.GetMinX(WorldRelation.Global) - Transform.GetMaxX(WorldRelation.Global)) / 2, 0, float.MaxValue);
                AfterglowY.Transform.SetSize(new Vector2(xSize, Transform.GetSize(WorldRelation.Global).X), WorldRelation.Global);
                AfterglowY.Transform.SetRotation(0);
            }
        }


        private void ShowXYAfterglow()
        {
            Vector2 thisPosition = Transform.GetPosition(WorldRelation.Global);
            Vector2 otherPosition = OtherPlayer.Transform.GetPosition(WorldRelation.Global);

            AfterglowMaterialY.AddShaderParameter("startTime", Time.Time.CurrentTime);
            
            AfterglowMaterialY.AddShaderParameter("direction", 1f);

            AfterglowY.Transform.SetPosition(new Vector2(otherPosition.X + 3 * (thisPosition.X - otherPosition.X) / 4, otherPosition.Y + 3 * (thisPosition.Y - otherPosition.Y) / 4), WorldRelation.Global);

            Components.Rectangle thisRect = Transform.GetGlobalRotatedRectangel();
            Components.Rectangle otherRect = OtherPlayer.Transform.GetGlobalRotatedRectangel();
            float xSize = System.Numerics.Vector2.Distance(new System.Numerics.Vector2(thisRect.BottomLeft.X, thisRect.BottomLeft.Y), new System.Numerics.Vector2(otherRect.BottomLeft.X, otherRect.BottomLeft.Y));
            AfterglowY.Transform.SetSize(new Vector2(xSize / 4, Transform.GetSize(WorldRelation.Global).X), WorldRelation.Global);

            float yDif = Transform.GetMaxY(WorldRelation.Global) - OtherPlayer.Transform.GetMaxY(WorldRelation.Global);
            float xDif = Transform.GetMaxX(WorldRelation.Global) - OtherPlayer.Transform.GetMaxX(WorldRelation.Global);
            AfterglowY.Transform.SetRotation(-MathHelper.RadiansToDegrees((float)Math.Atan2(yDif, xDif)));
        }

        protected abstract void HitSelectedBlock();
        

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
                ResetToLastCheckpoint();
                OtherPlayer.ResetToLastCheckpoint();
            }
        }

        public void ResetToLastCheckpoint()
        {
            Transform.SetPosition(CurrentFloor.FindStartPosition().GetPosition(WorldRelation.Global), WorldRelation.Global);
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