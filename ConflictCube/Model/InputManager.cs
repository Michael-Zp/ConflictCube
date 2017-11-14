using ConflictCube.Controller;
using OpenTK;
using System;
using System.Collections.Generic;

namespace ConflictCube.Model
{
    public class InputManager
    {
        public GameState State { get; private set; }
        public List<Player> Player { get; private set; }

        public InputManager(GameState state, List<Player> player)
        {
            State = state;
            Player = player;
        }

        public void ExecuteInputs(List<Input> inputs)
        {
            foreach(Input input in inputs)
            {
                switch (input)
                {
                    case Input.ExitApplication:
                        CloseGame();
                        break;
                }
                
                if (Player[0].IsAlive)
                {
                    switch(input)
                    {
                        case Input.PlayerOneMoveLeft:
                            MovePlayerLeft(0);
                            break;

                        case Input.PlayerOneMoveRight:
                            MovePlayerRight(0);
                            break;

                        case Input.PlayerOneMoveUp:
                            MovePlayerUp(0);
                            break;

                        case Input.PlayerOneMoveDown:
                            MovePlayerDown(0);
                            break;
                    }
                }
                
                if (Player[1].IsAlive)
                {
                    switch (input)
                    {
                        case Input.PlayerTwoMoveLeft:
                            MovePlayerLeft(1);
                            break;

                        case Input.PlayerTwoMoveRight:
                            MovePlayerRight(1);
                            break;

                        case Input.PlayerTwoMoveUp:
                            MovePlayerUp(1);
                            break;

                        case Input.PlayerTwoMoveDown:
                            MovePlayerDown(1);
                            break;
                    }
                }
            }
        }

        private void CloseGame()
        {
            Environment.Exit(0);
        }

        private void MovePlayerLeft(int idx)
        {
            State.MoveObject(Player[idx], new Vector2(Player[idx].Speed * -1, 0f));
        }

        private void MovePlayerRight(int idx)
        {
            State.MoveObject(Player[idx], new Vector2(Player[idx].Speed * 1, 0f));
        }

        private void MovePlayerUp(int idx)
        {
            State.MoveObject(Player[idx], new Vector2(.0f, Player[idx].Speed));
        }

        private void MovePlayerDown(int idx)
        {
            State.MoveObject(Player[idx], new Vector2(.0f, Player[idx].Speed * -1));
        }
    }
}
