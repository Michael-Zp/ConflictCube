using Engine.Components;
using Engine.Time;
using OpenTK;
using System.ComponentModel.Composition;

namespace ConflictCube.Objects
{
    public class PlayerSwitcher : GameObject, ISwitchPlayers
    {
#pragma warning disable 0649

        [Import(typeof(ITime))]
        private ITime Time;

#pragma warning restore 0649

        private Player PlayerOne;
        private Player PlayerTwo;

        private Vector2 PlayerOneTargetPos = new Vector2();
        private Vector2 PlayerTwoTargetPos = new Vector2();
        private bool SwitchPositions = false;
        private int StepCount = 0;
        private int MaxStepCount = 15;
        private float StepCooldown = 0.01f;
        private float LastStep = 0;

        public PlayerSwitcher(string name, Player playerOne, Player playerTwo, GameObject parent) : base(name, new Transform(), parent, true)
        {
            Program.Container.ComposeParts(this);

            PlayerOne = playerOne;
            PlayerTwo = playerTwo;

            LastStep = -StepCooldown;
        }

        public override void OnUpdate()
        {
            if (SwitchPositions && Time.CooldownIsOver(LastStep, StepCooldown))
            {
                LastStep = Time.CurrentTime;

                if (StepCount >= MaxStepCount - 1)
                {
                    PlayerOne.Transform.SetPosition(PlayerOneTargetPos, WorldRelation.Global);
                    PlayerTwo.Transform.SetPosition(PlayerTwoTargetPos, WorldRelation.Global);

                    DeactivateSwitching();
                }
                else
                {
                    Vector2 distanceToTargetPosPlayerOne = PlayerOneTargetPos - PlayerOne.Transform.GetPosition(WorldRelation.Global);
                    Vector2 distanceToTargetPosPlayerTwo = PlayerTwoTargetPos - PlayerTwo.Transform.GetPosition(WorldRelation.Global);

                    PlayerOne.Transform.SetPosition(PlayerOne.Transform.GetPosition(WorldRelation.Global) + distanceToTargetPosPlayerOne / 3, WorldRelation.Global);
                    PlayerTwo.Transform.SetPosition(PlayerTwo.Transform.GetPosition(WorldRelation.Global) + distanceToTargetPosPlayerTwo / 3, WorldRelation.Global);

                    StepCount++;
                }
            }
        }

        public void SwitchXAxis()
        {
            if (SwitchPositions)
            {
                return;
            }

            GetPlayerPositions(out Vector2 playerOnePos, out Vector2 playerTwoPos);

            PlayerOneTargetPos = new Vector2(playerTwoPos.X, playerOnePos.Y);
            PlayerTwoTargetPos = new Vector2(playerOnePos.X, playerTwoPos.Y);

            if (TargetsAreWalkable(PlayerOneTargetPos, PlayerTwoTargetPos))
            {
                ActivateSwitching();
            }
        }

        public void SwitchXYAxis()
        {
            if (SwitchPositions)
            {
                return;
            }

            GetPlayerPositions(out PlayerTwoTargetPos, out PlayerOneTargetPos);

            if (TargetsAreWalkable(PlayerOneTargetPos, PlayerTwoTargetPos))
            {
                ActivateSwitching();
            }
        }

        public void SwitchYAxis()
        {
            if (SwitchPositions)
            {
                return;
            }

            GetPlayerPositions(out Vector2 playerOnePos, out Vector2 playerTwoPos);

            PlayerOneTargetPos = new Vector2(playerOnePos.X, playerTwoPos.Y);
            PlayerTwoTargetPos = new Vector2(playerTwoPos.X, playerOnePos.Y);

            if (TargetsAreWalkable(PlayerOneTargetPos, PlayerTwoTargetPos))
            {
                ActivateSwitching();
            }
        }

        private void GetPlayerPositions(out Vector2 onePosition, out Vector2 twoPosition)
        {
            onePosition = PlayerOne.Transform.GetPosition(WorldRelation.Global);
            twoPosition = PlayerTwo.Transform.GetPosition(WorldRelation.Global);
        }

        private bool TargetsAreWalkable(Vector2 onePosition, Vector2 twoPosition)
        {
            return (PlayerOne.LevelTileIsWalkable(onePosition) && PlayerTwo.LevelTileIsWalkable(twoPosition));
        }

        private void ActivateSwitching()
        {
            SwitchPositions = true;
            StepCount = 0;

            ActivateSwitchingForPlayer(PlayerOne);
            ActivateSwitchingForPlayer(PlayerTwo);
        }

        private void ActivateSwitchingForPlayer(Player player)
        {
            player.CallOnUpdate = false;
            player.GetComponent<Collider>().IsTrigger = true;
            player.GetComponent<Material>().Color = System.Drawing.Color.FromArgb(128, player.GetComponent<Material>().Color);
        }

        private void DeactivateSwitching()
        {
            SwitchPositions = false;

            DeactivateSwitchingForPlayer(PlayerOne);
            DeactivateSwitchingForPlayer(PlayerTwo);
        }

        private void DeactivateSwitchingForPlayer(Player player)
        {
            player.CallOnUpdate = true;
            player.GetComponent<Collider>().IsTrigger = false;
            player.GetComponent<Material>().Color = System.Drawing.Color.FromArgb(255, player.GetComponent<Material>().Color);
        }
    }
}
