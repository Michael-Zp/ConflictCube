using ConflictCube.Controller;
using ConflictCube.Model.Renderable;
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

        /// <summary>
        ///     Switch all given inputs and call the functions which are defined as private in the input manager.
        ///     These functions either affect one of the players or the GameState.
        /// </summary>
        /// <param name="inputs">List of inputs to execute</param>
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

                        case Input.PlayerOneThrowMode:
                            SwitchThrowMode(0);
                            break;

                        case Input.PlayerOneUseMode:
                            SwitchUseMode(0);
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

                        case Input.PlayerOneThrowMode:
                            SwitchThrowMode(1);
                            break;

                        case Input.PlayerOneUseMode:
                            SwitchUseMode(1);
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
            Player[idx].Move(new Vector2(Player[idx].Speed * -1, 0f));
        }

        private void MovePlayerRight(int idx)
        {
            Player[idx].Move(new Vector2(Player[idx].Speed, 0f));
        }

        private void MovePlayerUp(int idx)
        {
            Player[idx].Move(new Vector2(.0f, Player[idx].Speed));
        }

        private void MovePlayerDown(int idx)
        {
            Player[idx].Move(new Vector2(.0f, Player[idx].Speed * -1));
        }

        private void SwitchThrowMode(int idx)
        {
            Player[idx].SwitchThrowMode();
        }

        private void SwitchUseMode(int idx)
        {
            Player[idx].SwitchUseMode();
        }
    }
}
