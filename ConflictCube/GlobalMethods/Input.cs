using System.Collections.Generic;
using OpenTK.Input;
using System;

namespace ConflictCube.ComponentBased
{
    public static class Input
    {
        public static Dictionary<Key, InputKey> KeyboardSettings = new Dictionary<Key, InputKey>();

        public static Dictionary<InputKey, bool> ButtonWasPressedDwon = new Dictionary<InputKey, bool>();
        public static Dictionary<InputKey, bool> ButtonIsPressed = new Dictionary<InputKey, bool>();
        public static Dictionary<InputKey, bool> ButtonWasReleased = new Dictionary<InputKey, bool>();

        private static KeyboardState LastState;


        static Input()
        {
            KeyboardSettings.Add(Key.Escape, InputKey.ExitApplication);

            //Player 1
            KeyboardSettings.Add(Key.A, InputKey.PlayerOneMoveLeft);
            KeyboardSettings.Add(Key.D, InputKey.PlayerOneMoveRight);
            KeyboardSettings.Add(Key.W, InputKey.PlayerOneMoveUp);
            KeyboardSettings.Add(Key.S, InputKey.PlayerOneMoveDown);
            KeyboardSettings.Add(Key.Q, InputKey.PlayerOneThrowMode);
            KeyboardSettings.Add(Key.E, InputKey.PlayerOneUseMode);


            //Player 2
            KeyboardSettings.Add(Key.J, InputKey.PlayerTwoMoveLeft);
            KeyboardSettings.Add(Key.L, InputKey.PlayerTwoMoveRight);
            KeyboardSettings.Add(Key.I, InputKey.PlayerTwoMoveUp);
            KeyboardSettings.Add(Key.K, InputKey.PlayerTwoMoveDown);
            KeyboardSettings.Add(Key.O, InputKey.PlayerTwoThrowMode);
            KeyboardSettings.Add(Key.U, InputKey.PlayerTwoUseMode);


            //Player 2 - Alt
            KeyboardSettings.Add(Key.Keypad4, InputKey.PlayerTwoMoveLeft);
            KeyboardSettings.Add(Key.Keypad6, InputKey.PlayerTwoMoveRight);
            KeyboardSettings.Add(Key.Keypad8, InputKey.PlayerTwoMoveUp);
            KeyboardSettings.Add(Key.Keypad5, InputKey.PlayerTwoMoveDown);
            KeyboardSettings.Add(Key.Keypad7, InputKey.PlayerTwoThrowMode);
            KeyboardSettings.Add(Key.Keypad9, InputKey.PlayerTwoUseMode);
        }


        public static void UpdateInputs()
        {
            foreach (Key key in KeyboardSettings.Keys)
            {
                bool keyIsPressed = Keyboard.GetState().IsKeyDown(key);
                bool lastIsKeyPressed = LastState.IsKeyDown(key);

                KeyboardSettings.TryGetValue(key, out InputKey input);

                if (keyIsPressed && !lastIsKeyPressed)
                {
                    SetInput(input, ButtonWasPressedDwon, true);
                }
                else if(keyIsPressed && lastIsKeyPressed)
                {
                    SetInput(input, ButtonWasPressedDwon, false);
                    SetInput(input, ButtonIsPressed, true);
                }
                else if(keyIsPressed && !lastIsKeyPressed)
                {
                    SetInput(input, ButtonIsPressed, false);
                    SetInput(input, ButtonWasReleased, true);
                }
                else if (!keyIsPressed && !lastIsKeyPressed)
                {
                    SetInput(input, ButtonWasReleased, false);

                }
            }
        }

        public static bool OnButtonDown(Key key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && !LastState.IsKeyDown(key));
        }

        public static bool OnButtonDown(InputKey input)
        {
            try
            {
                ButtonWasPressedDwon.TryGetValue(input, out bool state);
                return state;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static bool OnButtonIsPressed(Key key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && LastState.IsKeyDown(key));
        }

        public static bool OnButtonIsPressed(InputKey input)
        {
            try
            {
                ButtonIsPressed.TryGetValue(input, out bool state);
                return state;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool OnButtonIsReleased(Key key)
        {
            return (!Keyboard.GetState().IsKeyDown(key) && LastState.IsKeyDown(key));
        }

        public static bool OnButtonIsReleased(InputKey input)
        {
            try
            {
                ButtonWasReleased.TryGetValue(input, out bool state);
                return state;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void SetInput(InputKey input, Dictionary<InputKey, bool> mode, bool state)
        {
            if (mode.ContainsKey(input))
            {
                mode.Remove(input);
            }
            mode.Add(input, state);
        }
    }


}
