using ConflictCube.Controller;
using OpenTK;
using System;
using System.Collections.Generic;

namespace ConflictCube.Model
{
    public class InputManager
    {
        public Player Player { get; private set; }

        public InputManager(Player player)
        {
            Player = player;
        }

        public void ExecuteInputs(List<Input> inputs)
        {
            foreach(Input input in inputs)
            {
                switch(input)
                {
                    case Input.ExitApplication:
                        CloseGame();
                        break;

                    case Input.MoveLeft:
                        MovePlayerLeft();
                        break;

                    case Input.MoveRight:
                        MovePlayerRight();
                        break;

                    case Input.MoveUp:
                        MovePlayerUp();
                        break;

                    case Input.MoveDown:
                        MovePlayerDown();
                        break;
                }
            }
        }

        private void CloseGame()
        {
            Environment.Exit(0);
        }

        private void MovePlayerLeft()
        {
            Player.Move(new Vector2(Player.Speed * -1, 0f));
        }

        private void MovePlayerRight()
        {
            Player.Move(new Vector2(Player.Speed * 1, 0f));
        }

        private void MovePlayerUp()
        {
            Player.Move(new Vector2(.0f, Player.Speed));
        }

        private void MovePlayerDown()
        {
            Player.Move(new Vector2(.0f, Player.Speed * -1));
        }
    }
}
