using ConflictCube.ComponentBased.Components;
using OpenTK;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class PlayerSwitcher : GameObject, ISwitchPlayers
    {
        private Player PlayerOne;
        private Player PlayerTwo;
        private bool SwitchedThisFrame = false;

        public PlayerSwitcher(string name, Player playerOne, Player playerTwo, GameObject parent) : base(name, new Transform(), parent, true)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
        }

        public override void OnLateUpdate()
        {
            SwitchedThisFrame = false;
        }

        public void SwitchXAxis()
        {
            if(SwitchedThisFrame)
            {
                return;
            }

            GetPlayerPositions(out Vector2 onePosition, out Vector2 twoPosition);

            float temp = onePosition.X;
            onePosition.X = twoPosition.X;
            twoPosition.X = temp;

            ApplyPositionsIfTargetsAreWalkable(onePosition, twoPosition);
        }

        public void SwitchXYAxis()
        {
            if (SwitchedThisFrame)
            {
                return;
            }

            GetPlayerPositions(out Vector2 onePosition, out Vector2 twoPosition);
            
            ApplyPositionsIfTargetsAreWalkable(twoPosition, onePosition);
        }

        public void SwitchYAxis()
        {
            if (SwitchedThisFrame)
            {
                return;
            }

            GetPlayerPositions(out Vector2 onePosition, out Vector2 twoPosition);

            float temp = onePosition.Y;
            onePosition.Y = twoPosition.Y;
            twoPosition.Y = temp;
            
            ApplyPositionsIfTargetsAreWalkable(onePosition, twoPosition);
        }

        private void GetPlayerPositions(out Vector2 onePosition, out Vector2 twoPosition)
        {
            onePosition = PlayerOne.Transform.GetPosition(WorldRelation.Global);
            twoPosition = PlayerTwo.Transform.GetPosition(WorldRelation.Global);
        }

        private void ApplyPositionsIfTargetsAreWalkable(Vector2 onePosition, Vector2 twoPosition)
        {
            if (PlayerOne.LevelTileIsWalkable(onePosition) && PlayerTwo.LevelTileIsWalkable(twoPosition))
            {
                PlayerOne.Transform.SetPosition(onePosition, WorldRelation.Global);
                PlayerTwo.Transform.SetPosition(twoPosition, WorldRelation.Global);
            }
        }
    }
}
