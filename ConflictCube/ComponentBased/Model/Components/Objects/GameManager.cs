using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.UI;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class GameManager : GameObject, IGameManager
    {
        private List<Player> Players;
        private GameOverScreen GameOverScreen;
        private GameWonScreen GameWonScreen;
        private IBuildMenu BuildMenu;

        public GameManager(string name, List<Player> players, IBuildMenu buildMenu, GameObject parent) : base(name, new Transform(), parent)
        {
            Players = players;
            BuildMenu = buildMenu;

            GameOverScreen = new GameOverScreen("GameOverScreen", new Transform(), this, false);
            GameWonScreen = new GameWonScreen("GameWonScreen", new Transform(), this, false);

            GameOverScreen.Parent = this;
            GameWonScreen.Parent = this;
        }

        public override void OnUpdate()
        {
            if(!CheckLooseCondition())
            {
                CheckWinCondition();
            }
        }


        private bool CheckLooseCondition()
        {
            if (!DebugGame.CanLoose)
            {
                return false;
            }

            bool playersAreAlive = true;

            foreach (Player player in Players)
            {
                playersAreAlive &= player.IsAlive;
            }

            if (!playersAreAlive)
            {
                foreach (Player player in Players)
                {
                    player.IsAlive = false;
                }

                GameOverScreen.Enabled = true;

                if (Input.AnyButtonDown())
                {
                    foreach (Player player in Players)
                    {
                        player.ResetPositionToLastCheckpoint();
                        player.IsAlive = true;
                    }

                    GameOverScreen.Enabled = false;
                }

                return true;
            }

            return false;
        }

        private void CheckWinCondition()
        {
            bool levelFinished = true;

            foreach (Player player in Players)
            {
                levelFinished &= player.GetTypeOfFloortileAtPlayerPos() == GameObjectType.Finish;
            }

            if (levelFinished)
            {
                foreach(Player player in Players)
                {
                    player.CanMove = false;
                }

                GameWonScreen.Enabled = true;

                if (Input.AnyButtonDown())
                {
                    ShowMenu();

                    GameOverScreen.Enabled = false;
                }
            }
        }


        public void HideDeathScreen()
        {
            GameOverScreen.Enabled = false;
        }

        public void ShowDeathScreen()
        {
            GameOverScreen.Enabled = true;
        }

        public void ShowMenu()
        {
            BuildMenu.BuildMenu();
        }

        public void ShowWonScreen()
        {
            GameWonScreen.Enabled = true;
        }

        public void HideWonScreen()
        {
            GameWonScreen.Enabled = false;
        }

        public void SetDeathReason(string reason)
        {
            GameOverScreen.SetDeathReason(reason);
        }
    }
}
