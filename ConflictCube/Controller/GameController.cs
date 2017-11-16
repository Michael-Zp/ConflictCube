using System.Collections.Generic;
using OpenTK.Input;

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

            //Player 1
            KeyboardSettings.Add(Key.A,      Input.PlayerOneMoveLeft);
            KeyboardSettings.Add(Key.D,      Input.PlayerOneMoveRight);
            KeyboardSettings.Add(Key.W,      Input.PlayerOneMoveUp);
            KeyboardSettings.Add(Key.S,      Input.PlayerOneMoveDown);


            //Player 2
            KeyboardSettings.Add(Key.Left,   Input.PlayerTwoMoveLeft);
            KeyboardSettings.Add(Key.Right,  Input.PlayerTwoMoveRight);
            KeyboardSettings.Add(Key.Up,     Input.PlayerTwoMoveUp);
            KeyboardSettings.Add(Key.Down,   Input.PlayerTwoMoveDown);
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
