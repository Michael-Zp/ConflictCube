using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictCube.Model
{
    public class InputManager : IInputManager
    {
        public GameView View { get; set; }
        public Player Player { get; set; }

        public InputManager(GameView view)
        {
            View = view;
        }

        public void CloseGame()
        {
            View.CloseWindow();
        }

        public void MovePlayerDown()
        {
            Player.Move(new Vector2(.0f, Player.Speed * -1));
        }

        public void MovePlayerUp()
        {
            Player.Move(new Vector2(.0f, Player.Speed));
        }

        public void MovePlayerRight()
        {
            Player.Move(new Vector2(Player.Speed * 1, 0f));
        }

        public void MovePlayerLeft()
        {
            Player.Move(new Vector2(Player.Speed * -1, 0f));
        }
    }
}
