using OpenTK;
using System;
using ConflictCube.ComponentBased.Components;
using System.Collections.Generic;
using ConflictCube.ComponentBased.Model.Components.Objects;

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
        public PlayerInventory Inventory = new PlayerInventory(1);

        private GameObject ThrowUseField { get; set; }
        /// <summary>
        ///     Wait time until the ThrowUseField vector can be updated again.
        /// </summary>
        private float ThrowUseFieldUpdateCooldown = 0.1f;
        private float LastThrowUseFieldUpdate { get; set; }
        private int CurrentFloor;
        private List<Floor> Floors;

        private static bool MaterialsAreInitialized = false;
        private static Material UseMaterial;
        private static Material ThrowMaterial;
        private float ThrowUseXOffset = 0;
        private float ThrowUseYOffset = 0;

        private InputAxis Horizontal;
        private InputAxis Vertical;
        private InputKey ThrowUseUp;
        private InputKey ThrowUseDown;
        private InputKey ThrowUseLeft;
        private InputKey ThrowUseRight;
        private InputKey Sprint;
        private InputKey SwitchMode;
        private InputKey Use;



        /// <summary>
        ///     Create a new player, with a defined size, position and move speed. The collision box of this player equals his rendering box, given with size and position
        /// </summary>
        public Player(string name, Transform transform, BoxCollider boxCollider, Material material, GameObject parent, int currentFloor, List<Floor> floors, float speed, GameObjectType playerType, bool isAlive = true) : base(name, transform, parent, playerType)
        {
            Speed = speed;
            IsAlive = isAlive;
            Floors = floors;
            CurrentFloor = currentFloor;
            LastThrowUseFieldUpdate = -ThrowUseFieldUpdateCooldown;

            AddComponent(boxCollider);
            AddComponent(material);

            if (!MaterialsAreInitialized)
            {
                MaterialsAreInitialized = true;
                UseMaterial = new Material(null, null, System.Drawing.Color.FromArgb(128, 255, 0, 0));
                ThrowMaterial = new Material(null, null, System.Drawing.Color.FromArgb(128, 0, 0, 255));
            }

            ThrowUseField = new ColoredBox("ThrowUseIndicator", new Transform(), UseMaterial, this, false);
            Floors[CurrentFloor].AddChild(ThrowUseField);
            SetThrowUseFieldWithOffset();

            switch (playerType)
            {
                case GameObjectType.Player1:
                    Horizontal = InputAxis.Player1Horizontal;
                    Vertical = InputAxis.Player1Vertical;
                    ThrowUseUp = InputKey.PlayerOneMoveThrowUseFieldUp;
                    ThrowUseDown = InputKey.PlayerOneMoveThrowUseFieldDown;
                    ThrowUseLeft = InputKey.PlayerOneMoveThrowUseFieldLeft;
                    ThrowUseRight = InputKey.PlayerOneMoveThrowUseFieldRight;
                    Sprint = InputKey.PlayerOneSprint;
                    SwitchMode = InputKey.PlayerOneSwitchMode;
                    Use = InputKey.PlayerOneUse;
                    break;

                case GameObjectType.Player2:
                    Horizontal = InputAxis.Player2Horizontal;
                    Vertical = InputAxis.Player2Vertical;
                    ThrowUseUp = InputKey.PlayerTwoMoveThrowUseFieldUp;
                    ThrowUseDown = InputKey.PlayerTwoMoveThrowUseFieldDown;
                    ThrowUseLeft = InputKey.PlayerTwoMoveThrowUseFieldLeft;
                    ThrowUseRight = InputKey.PlayerTwoMoveThrowUseFieldRight;
                    Sprint = InputKey.PlayerTwoSprint;
                    SwitchMode = InputKey.PlayerTwoSwitchMode;
                    Use = InputKey.PlayerTwoUse;
                    break;
            }
        }

        public override void OnUpdate()
        {
            Vector2 moveVector = new Vector2(Input.GetAxis(Horizontal), Input.GetAxis(Vertical));

            if ((Input.OnButtonIsPressed(Sprint) || Input.OnButtonDown(Sprint)) && CurrentSprintEnergy > UsedSprintEnergyPerSecond * Time.Time.DifTime)
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

            if (Input.OnButtonDown(SwitchMode))
            {
                SwitchBetweenModes();
            }

            if (Input.OnButtonDown(Use))
            {
                ThrowOrUseBlock();
            }

            Move(moveVector);
            UpdateThrowUseField();
        }

        private void ThrowOrUseBlock()
        {
            if (UseMode || ThrowMode)
            {
                Vector2 currentPosOfThrowUseField = Floors[CurrentFloor].GetGridPosition(Transform.TransformToGlobal()) + new Vector2(ThrowUseXOffset, ThrowUseYOffset);

                int indexOfColumn = (int)currentPosOfThrowUseField.X;
                int indexOfRow = Floors[CurrentFloor].FloorRows - 1 - (int)currentPosOfThrowUseField.Y;

                if (UseMode && Inventory.Cubes > 0)
                {
                    Floors[CurrentFloor].FloorTiles[indexOfRow, indexOfColumn].ChangeFloorTile(GameObjectType.Wall);
                    Inventory.Cubes -= 1;
                }
                else if (ThrowMode && Inventory.Cubes > 0)
                {
                    for (int i = 0; i < Floors.Count; i++)
                    {
                        if (i == CurrentFloor)
                        {
                            continue;
                        }
                        Floors[i].FloorTiles[indexOfRow, indexOfColumn].ChangeFloorTile(GameObjectType.Wall);
                    }
                    Inventory.Cubes -= 1;
                }
            }
        }

        private void UpdateThrowUseField()
        {
            if (Time.Time.CooldownIsOver(LastThrowUseFieldUpdate, ThrowUseFieldUpdateCooldown) && ThrowMode || UseMode)
            {
                LastThrowUseFieldUpdate = Time.Time.CurrentTime;

                if (Input.OnButtonDown(ThrowUseUp))
                {
                    ThrowUseYOffset += 1;
                }
                if (Input.OnButtonDown(ThrowUseDown))
                {
                    ThrowUseYOffset -= 1;
                }
                if (Input.OnButtonDown(ThrowUseLeft))
                {
                    ThrowUseXOffset -= 1;
                }
                if (Input.OnButtonDown(ThrowUseRight))
                {
                    ThrowUseXOffset += 1;
                }

                ThrowUseXOffset = MathHelper.Clamp(ThrowUseXOffset, -1, 1);
                ThrowUseYOffset = MathHelper.Clamp(ThrowUseYOffset, -1, 1);

                try
                {
                    SetThrowUseFieldWithOffset(ThrowUseXOffset, ThrowUseYOffset);
                }
                catch (Exception)
                {
                    Vector2 currentPos = Floors[CurrentFloor].GetGridPosition(Transform.TransformToGlobal()) + new Vector2(ThrowUseXOffset, ThrowUseYOffset);

                    if (currentPos.X > Floors[CurrentFloor].FloorColumns - 1)
                    {
                        ThrowUseXOffset -= 1;
                    }
                    else if (currentPos.X < 0)
                    {
                        ThrowUseXOffset += 1;
                    }

                    if (currentPos.Y > Floors[CurrentFloor].FloorRows - 1)
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
            if (other.Type == CollisionType.LeftBoundary ||
                other.Type == CollisionType.RightBoundary ||
                other.Type == CollisionType.TopBoundary ||
                other.Type == CollisionType.BottomBoundary ||
                other.Type == CollisionType.Wall)
            {

            }
            else if (other.Type == CollisionType.Hole)
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
        public void SwitchBetweenModes()
        {
            if (!ThrowMode && !UseMode)
            {
                ThrowUseXOffset = 0;
                ThrowUseYOffset = 0;
                UseMode = true;
                ThrowUseField.Enabled = true;
                ThrowUseField.RemoveComponent<Material>();
                ThrowUseField.AddComponent(UseMaterial);
            }
            else if (UseMode)
            {
                UseMode = false;
                ThrowMode = true;
                ThrowUseField.RemoveComponent<Material>();
                ThrowUseField.AddComponent(ThrowMaterial);
            }
            else if (ThrowMode)
            {
                ThrowMode = false;
                ThrowUseField.Enabled = false;
            }
        }

        private void SetThrowUseFieldWithOffset(float xOffset = 0, float yOffset = 0)
        {
            Vector2 currentPos = Floors[CurrentFloor].GetGridPosition(Transform.TransformToGlobal()) + new Vector2(xOffset, yOffset);
            Transform throwUseTransform = Floors[CurrentFloor].GetBoxAtGridPosition(currentPos);
            Transform throwUseLocalTransform = Floors[CurrentFloor].Transform.TransformToLocal(throwUseTransform);
            ThrowUseField.Transform.Position = throwUseLocalTransform.Position;
            ThrowUseField.Transform.Size = throwUseLocalTransform.Size;
        }
    }
}