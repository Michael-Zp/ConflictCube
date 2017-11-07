using System.Collections.Generic;
using OpenTK.Input;
using System;
using ConflictCube.Model;

namespace ConflictCube.Controller
{
    public class GameController
    {
        private Dictionary<Key, Input> KeyboardSettings = new Dictionary<Key, Input>();

        public GameController()
        {
            SetDefaultKeyboardSettings();
        }

        private void SetDefaultKeyboardSettings()
        {
            KeyboardSettings.Add(Key.Escape, Input.ExitApplication);
            KeyboardSettings.Add(Key.Left,   Input.MoveLeft);
            KeyboardSettings.Add(Key.Right,  Input.MoveRight);
            KeyboardSettings.Add(Key.Up,     Input.MoveUp);
            KeyboardSettings.Add(Key.Down,   Input.MoveDown);
        }

        public void UpdateState()
        {
            //Lookup in some kind of structure which methods are called if a button is pressed
            //e.g if esc is pressed close the window
            
        }

        public List<Input> GetInputs()
        {
            List<Input> inputs = new List<Input>();
            foreach (Key key in KeyboardSettings.Keys)
            {
                if (Keyboard.GetState().IsKeyDown(key))
                {
                    Input input;
                    KeyboardSettings.TryGetValue(key, out input);

                    inputs.Add(input);
                }
            }

            return inputs;
        }
    }
}
