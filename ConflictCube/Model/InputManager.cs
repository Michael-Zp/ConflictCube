using ConflictCube.Controller;
using OpenTK;
using System;
using System.Collections.Generic;

namespace ConflictCube.Model
{
    public class InputManager
    {
        public GameState State { get; private set; }
        public Player Player { get; private set; }

        public InputManager(GameState state, Player player)
        {
            State = state;
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
            State.MoveObject(Player, new Vector2(Player.Speed * -1, 0f));
        }

        private void MovePlayerRight()
        {
            State.MoveObject(Player, new Vector2(Player.Speed * 1, 0f));
        }

        private void MovePlayerUp()
        {
            State.MoveObject(Player, new Vector2(.0f, Player.Speed));
        }

        private void MovePlayerDown()
        {
            State.MoveObject(Player, new Vector2(.0f, Player.Speed * -1));
        }
    }
}
