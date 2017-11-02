using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictCube
{
    public class GameState
    {
        private GameView View;

        public Level CurrentLevel { get; set; }

        public GameState(GameView view)
        {
            this.View = view;
        }

        public void Render()
        {
            View.ClearScreen();
            View.ShowLevel(CurrentLevel);
        }

        public void LoadLevel(int levelNumber)
        {
            CurrentLevel = LevelBuilder.LoadLevel(levelNumber);
        }

        public void CloseGame()
        {
            View.CloseWindow();
        }

        public void NextFrame()
        {

        }
    }
}
