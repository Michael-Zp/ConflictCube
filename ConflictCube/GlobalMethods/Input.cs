using System.Collections.Generic;
using OpenTK.Input;
using System;
using ConflictCube.GlobalMethods;

namespace ConflictCube.ComponentBased
{
    public enum InputAxis
    {
        Player1Horizontal,
        Player1Vertical,
        Player2Horizontal,
        Player2Vertical,
    }

    public struct AxisData
    {
        public InputKey PositiveKey;
        public InputKey NegativeKey;

        public AxisData(InputKey positiveKey, InputKey negativeKey)
        {
            PositiveKey = positiveKey;
            NegativeKey = negativeKey;
        }
    }

    public static class Input
    {
        public static Dictionary<InputKey, Key> KeyboardSettings = new Dictionary<InputKey, Key>();
        public static Dictionary<InputAxis, float> Axes = new Dictionary<InputAxis, float>();
        public static Dictionary<InputAxis, AxisData> AxesSettings = new Dictionary<InputAxis, AxisData>();

        public static Dictionary<InputKey, GamePadButton> GamePadSettings = new Dictionary<InputKey, GamePadButton>();
        public static Dictionary<InputAxis, GamePadAxis> GamePadAxesSettings = new Dictionary<InputAxis, GamePadAxis>();


        private static bool KeyboardState1IsCurrent = true;
        private static KeyboardState _KeyboardState1 = new KeyboardState();
        private static KeyboardState _KeyboardState2 = new KeyboardState();
        private static KeyboardState CurrentKeyboardState {
            get {
                if (KeyboardState1IsCurrent)
                {
                    return _KeyboardState1;
                }
                else
                {
                    return _KeyboardState2;
                }
            }
            set {
                if(KeyboardState1IsCurrent)
                {
                    _KeyboardState1 = value;
                }
                else
                {
                    _KeyboardState2 = value;
                }
            }
        }
        private static KeyboardState LastKeyboardState {
            get {
                if(KeyboardState1IsCurrent)
                {
                    return _KeyboardState2;
                }
                else
                {
                    return _KeyboardState1;
                }
            }
        }
        /// <summary>
        /// Keyboard states can not be cloned, so a normal LastKeyboardState = CurrentKeyboardState will not work, because the two states are now the same.
        /// Use a flag KeyboardState1IsCurrent and two KeyboardStates. Swap these KeyboardStates between Current and Last with this flag (see getters) and only set the current state.
        /// Kind of hacky but it works.
        /// </summary>
        private static void UpdateKeyboardStates()
        {
            KeyboardState1IsCurrent = !KeyboardState1IsCurrent;
            CurrentKeyboardState = Keyboard.GetState();
        }


        private static bool GamePadState1IsCurrent = true;
        private static List<GamePadState> _GamePadState1 = new List<GamePadState>(GamePadCount);
        private static List<GamePadState> _GamePadState2 = new List<GamePadState>(GamePadCount);
        private static List<GamePadState> CurrentGamePadState {
            get {
                if (GamePadState1IsCurrent)
                {
                    return _GamePadState1;
                }
                else
                {
                    return _GamePadState2;
                }
            }
            set {
                if (GamePadState1IsCurrent)
                {
                    _GamePadState1 = value;
                }
                else
                {
                    _GamePadState2 = value;
                }
            }
        }
        private static List<GamePadState> LastGamePadState {
            get {
                if (GamePadState1IsCurrent)
                {
                    return _GamePadState2;
                }
                else
                {
                    return _GamePadState1;
                }
            }
        }
        /// <summary>
        /// Keyboard states can not be cloned, so a normal LastGamePadState = CurrentGamePadState will not work, because the two states are now the same.
        /// Use a flag GamePadState1IsCurrent and two GamePadStates. Swap these GamePadStates between Current and Last with this flag (see getters) and only set the current state.
        /// Kind of hacky but it works.
        /// </summary>
        private static void UpdateGamePadStates()
        {
            GamePadState1IsCurrent = !GamePadState1IsCurrent;

            List<GamePadState> currentStates = new List<GamePadState>(GamePadCount);

            for (int i = 0; i < GamePadCount; i++)
            {
                currentStates.Add(GamePad.GetState(i));
            }

            CurrentGamePadState = currentStates;
        }

        private const int GamePadCount = 4;


        static Input()
        {
            //GamePad and Keyboard - General

            KeyboardSettings.Add(InputKey.ExitApplication, Key.Escape);
            GamePadSettings.Add(InputKey.ExitApplication, GamePadButton.RightStick);


            //GamePad - Player 1
            
            GamePadSettings.Add(InputKey.PlayerOneSprint, GamePadButton.RightShoulder);
            GamePadSettings.Add(InputKey.PlayerOneUse, GamePadButton.X);
            GamePadSettings.Add(InputKey.PlayerOneSwitchPositionsX, GamePadButton.Y);
            GamePadSettings.Add(InputKey.PlayerOneSwitchPositionsXY, GamePadButton.A);
            GamePadSettings.Add(InputKey.PlayerOneSwitchPositionsY, GamePadButton.B);

            //Player 2

            GamePadSettings.Add(InputKey.PlayerTwoSprint, GamePadButton.RightShoulder);
            GamePadSettings.Add(InputKey.PlayerTwoUse, GamePadButton.X);
            GamePadSettings.Add(InputKey.PlayerTwoSwitchPositionsX, GamePadButton.Y);
            GamePadSettings.Add(InputKey.PlayerTwoSwitchPositionsXY, GamePadButton.A);
            GamePadSettings.Add(InputKey.PlayerTwoSwitchPositionsY, GamePadButton.B);


            //Gamepad - Axes - Player 1

            GamePadAxesSettings.Add(InputAxis.Player1Horizontal, GamePadAxis.LeftPadX);
            GamePadAxesSettings.Add(InputAxis.Player1Vertical, GamePadAxis.LeftPadY);
            
            //Player 2

            GamePadAxesSettings.Add(InputAxis.Player2Horizontal, GamePadAxis.LeftPadX);
            GamePadAxesSettings.Add(InputAxis.Player2Vertical, GamePadAxis.LeftPadY);

            for(int i = 0; i < GamePadCount; i++)
            {
                LastGamePadState.Add(GamePad.GetState(i));
            }

            //Keyboard - Player 1
            KeyboardSettings.Add(InputKey.PlayerOneMoveLeft, Key.A);
            KeyboardSettings.Add(InputKey.PlayerOneMoveRight, Key.D);
            KeyboardSettings.Add(InputKey.PlayerOneMoveUp, Key.W);
            KeyboardSettings.Add(InputKey.PlayerOneMoveDown, Key.S);
            KeyboardSettings.Add(InputKey.PlayerOneSprint, Key.LShift);
            KeyboardSettings.Add(InputKey.PlayerOneUse, Key.E);
            KeyboardSettings.Add(InputKey.PlayerOneSwitchPositionsY, Key.R);
            KeyboardSettings.Add(InputKey.PlayerOneSwitchPositionsXY, Key.Q);
            KeyboardSettings.Add(InputKey.PlayerOneSwitchPositionsX, Key.F);
            KeyboardSettings.Add(InputKey.Zoom, Key.Space);




            //Player 2
            KeyboardSettings.Add(InputKey.PlayerTwoMoveLeft, Key.J);
            KeyboardSettings.Add(InputKey.PlayerTwoMoveRight, Key.L);
            KeyboardSettings.Add(InputKey.PlayerTwoMoveUp, Key.I);
            KeyboardSettings.Add(InputKey.PlayerTwoMoveDown, Key.K);
            KeyboardSettings.Add(InputKey.PlayerTwoSprint, Key.RControl);
            KeyboardSettings.Add(InputKey.PlayerTwoUse, Key.O);
            KeyboardSettings.Add(InputKey.PlayerTwoSwitchPositionsY, Key.Y); //Uses QWERTY layout
            KeyboardSettings.Add(InputKey.PlayerTwoSwitchPositionsXY, Key.U);
            KeyboardSettings.Add(InputKey.PlayerTwoSwitchPositionsX, Key.H);


            //Axes
            AxesSettings.Add(InputAxis.Player1Horizontal, new AxisData(InputKey.PlayerOneMoveRight, InputKey.PlayerOneMoveLeft));
            AxesSettings.Add(InputAxis.Player1Vertical, new AxisData(InputKey.PlayerOneMoveUp, InputKey.PlayerOneMoveDown));
            AxesSettings.Add(InputAxis.Player2Horizontal, new AxisData(InputKey.PlayerTwoMoveRight, InputKey.PlayerTwoMoveLeft));
            AxesSettings.Add(InputAxis.Player2Vertical, new AxisData(InputKey.PlayerTwoMoveUp, InputKey.PlayerTwoMoveDown));


            Axes.Add(InputAxis.Player1Horizontal, 0f);
            Axes.Add(InputAxis.Player1Vertical, 0f);
            Axes.Add(InputAxis.Player2Horizontal, 0f);
            Axes.Add(InputAxis.Player2Vertical, 0f);
        }






        // Update ----

        public static void UpdateInputs()
        {
            UpdateKeyboard();

            UpdateGamePadStates();
        }

        private static void UpdateKeyboard()
        {
            UpdateAxes();

            UpdateKeyboardStates();
        }

        private static void UpdateAxes()
        {
            UpdateAxis(InputAxis.Player1Horizontal);
            UpdateAxis(InputAxis.Player1Vertical);
            UpdateAxis(InputAxis.Player2Horizontal);
            UpdateAxis(InputAxis.Player2Vertical);
        }

        private static void UpdateAxis(InputAxis axis)
        {
            AxesSettings.TryGetValue(axis, out AxisData data);

            bool positivePressed = OnButtonIsPressed(data.PositiveKey) || OnButtonDown(data.PositiveKey);
            bool negativePressed = OnButtonIsPressed(data.NegativeKey) || OnButtonDown(data.NegativeKey);

            Axes.TryGetValue(axis, out float currentValue);
            
            if (!positivePressed && !negativePressed)
            {
                currentValue = 0;
            }
            else if (!positivePressed && negativePressed)
            {
                if (currentValue > 0)
                {
                    currentValue = 0;
                }
                currentValue -= .1f;
            }
            else if (positivePressed && !negativePressed)
            {
                if (currentValue < 0)
                {
                    currentValue = 0;
                }
                currentValue += .1f;
            }
            else if (positivePressed && negativePressed)
            {
                currentValue = 0;
            }

            currentValue = Zenseless.Geometry.MathHelper.Clamp(currentValue, -1, 1);

            Axes.Remove(axis);
            Axes.Add(axis, currentValue);
        }






        // Get Key states

        private static bool FetchKeyboardKey(InputKey input, out Key key)
        {
            if (!KeyboardSettings.ContainsKey(input))
            {
                key = Key.F35;
                return false;
            }
            KeyboardSettings.TryGetValue(input, out key);
            return true;
        }

        private static bool FetchGamepadKey(InputKey input, out GamePadButton button)
        {
            if(!GamePadSettings.ContainsKey(input))
            {
                button = GamePadButton.None;
                return false;
            }
            GamePadSettings.TryGetValue(input, out button);
            return true;
        }

        public static bool OnGamePadButtonDown(GamePadButton button, int activeGamePad)
        {
            return GamePad.GetState(activeGamePad).IsPressed(button) && !LastGamePadState[activeGamePad].IsPressed(button);
        }

        public static bool OnButtonDown(Key key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && !LastKeyboardState.IsKeyDown(key);
        }

        public static bool OnButtonDown(InputKey input, int activeGamePad = 0)
        {
            bool buttonDown = false;

            if(FetchKeyboardKey(input, out Key KeyboardKey))
            {
                buttonDown = buttonDown || OnButtonDown(KeyboardKey);
            }

            if (FetchGamepadKey(input, out GamePadButton gamePadButton))
            {
                buttonDown = buttonDown || OnGamePadButtonDown(gamePadButton, activeGamePad);
            }

            return buttonDown;
        }


        public static bool OnGamePadButtonIsPressed(GamePadButton button, int activeGamePad)
        {
            return GamePad.GetState(activeGamePad).IsPressed(button) && LastGamePadState[activeGamePad].IsPressed(button);
        }

        public static bool OnButtonIsPressed(Key key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyDown(key);
        }

        public static bool OnButtonIsPressed(InputKey input, int activeGamePad = 0)
        {
            bool buttonPressed = false;

            if (FetchKeyboardKey(input, out Key KeyboardKey))
            {
                buttonPressed = buttonPressed || OnButtonIsPressed(KeyboardKey);
            }

            if (FetchGamepadKey(input, out GamePadButton gamePadButton))
            {
                buttonPressed = buttonPressed || OnGamePadButtonIsPressed(gamePadButton, activeGamePad);
            }

            return buttonPressed;
        }

        
        public static bool OnGamePadButtonIsReleased(GamePadButton button, int activeGamePad)
        {
            return !GamePad.GetState(activeGamePad).IsPressed(button) && LastGamePadState[activeGamePad].IsPressed(button);
        }

        public static bool OnButtonIsReleased(Key key)
        {
            return !CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyDown(key);
        }

        public static bool OnButtonIsReleased(InputKey input, int activeGamePad = 0)
        {
            bool buttonReleased = false;

            if (FetchKeyboardKey(input, out Key KeyboardKey))
            {
                buttonReleased = buttonReleased || OnButtonIsReleased(KeyboardKey);
            }

            if (FetchGamepadKey(input, out GamePadButton gamePadButton))
            {
                buttonReleased = buttonReleased || OnGamePadButtonIsReleased(gamePadButton, activeGamePad);
            }

            return buttonReleased;
        }





        // Get Axes

        public static float GetAxis(InputAxis axis, int activeGamePad)
        {
            float keyboardAxis = GetKeyboardAxis(axis);
            float gamepadAxis = GetGamePadAxis(axis, activeGamePad);
            return Math.Abs(keyboardAxis) > Math.Abs(gamepadAxis) ? keyboardAxis : gamepadAxis;
        }

        public static float GetKeyboardAxis(InputAxis axis)
        {
            Axes.TryGetValue(axis, out float currentValue);

            return currentValue;
        }
        
        public static float GetGamePadAxis(InputAxis axis, int activeGamePad)
        {
            GamePadAxesSettings.TryGetValue(axis, out GamePadAxis gpAxis);
            return (float)Math.Round(GamePad.GetState(activeGamePad).GetAxis(gpAxis), 2);
        }
    }


}
