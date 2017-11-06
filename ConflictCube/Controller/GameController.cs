using System.Collections.Generic;
using OpenTK.Input;
using System;
using ConflictCube.Model;

namespace ConflictCube
{
    public class GameController
    {
        private Dictionary<Key, Action> KeyboardSettings = new Dictionary<Key, Action>();
        private GameState State;

        public GameController(GameState State)
        {
            this.State = State;
            SetDefaultKeyboardSettings();
        }

        private void SetDefaultKeyboardSettings()
        {
            KeyboardSettings.Add(Key.Escape, State.InputManager.CloseGame);
            KeyboardSettings.Add(Key.Left, State.InputManager.MovePlayerLeft);
            KeyboardSettings.Add(Key.Right, State.InputManager.MovePlayerRight);
            KeyboardSettings.Add(Key.Up, State.InputManager.MovePlayerUp);
            KeyboardSettings.Add(Key.Down, State.InputManager.MovePlayerDown);
        }

        public void UpdateState()
        {
            //Lookup in some kind of structure which methods are called if a button is pressed
            //e.g if esc is pressed close the window
            foreach (Key key in KeyboardSettings.Keys)
            {
                if (Keyboard.GetState().IsKeyDown(key))
                {
                    Action keyAction;
                    KeyboardSettings.TryGetValue(key, out keyAction);

                    keyAction?.Invoke();
                }
            }
        }

        public void LoadLevel(int level)
        {
            State.LoadLevel(level);
        }

        public void InitializePlayer()
        {
            State.InitializePlayer();
        }
    }
}
