using System.Collections.Generic;
using OpenTK.Input;
using System;

namespace Engine.Inputs
{
    public static class Input
    {
        public static Dictionary<string, Key> KeyboardSettings = new Dictionary<string, Key>();
        public static Dictionary<string, float> Axes = new Dictionary<string, float>();
        public static Dictionary<string, AxisData> AxesSettings = new Dictionary<string, AxisData>();

        public static Dictionary<string, GamePadButton> GamePadSettings = new Dictionary<string, GamePadButton>();
        public static Dictionary<string, GamePadAxis> GamePadAxesSettings = new Dictionary<string, GamePadAxis>();


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
                if (KeyboardState1IsCurrent)
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
                if (KeyboardState1IsCurrent)
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

        private const int GamePadCount = 2;
        
        static Input()
        {
            for (int i = 0; i < Input.GamePadCount; i++)
            {
                Input.LastGamePadState.Add(GamePad.GetState(i));
            }
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
            List<string> axesKeys = new List<string>();

            foreach(string key in Axes.Keys)
            {
                axesKeys.Add(key);
            }

            for(int i = 0; i < Axes.Count; i++)
            {
                UpdateAxis(axesKeys[i]);
            }
        }

        private static void UpdateAxis(string axis)
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

        private static bool FetchKeyboardKey(string input, out Key key)
        {
            if (!KeyboardSettings.ContainsKey(input))
            {
                key = Key.F35;
                return false;
            }
            KeyboardSettings.TryGetValue(input, out key);
            return true;
        }

        private static bool FetchGamepadKey(string input, out GamePadButton button)
        {
            if (!GamePadSettings.ContainsKey(input))
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

        public static bool AnyButtonDown()
        {
            return CurrentKeyboardState.IsAnyKeyDown && !LastKeyboardState.IsAnyKeyDown;
        }

        public static bool OnButtonDown(Key key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && !LastKeyboardState.IsKeyDown(key);
        }

        public static bool OnButtonDown(string input, int activeGamePad = 0)
        {
            bool buttonDown = false;

            if (FetchKeyboardKey(input, out Key KeyboardKey))
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

        public static bool OnButtonIsPressed(string input, int activeGamePad = 0)
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

        public static bool OnButtonIsReleased(string input, int activeGamePad = 0)
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

        public static float GetAxis(string axis, int activeGamePad)
        {
            float keyboardAxis = GetKeyboardAxis(axis);
            float gamepadAxis = GetGamePadAxis(axis, activeGamePad);
            return Math.Abs(keyboardAxis) > Math.Abs(gamepadAxis) ? keyboardAxis : gamepadAxis;
        }

        public static float GetKeyboardAxis(string axis)
        {
            Axes.TryGetValue(axis, out float currentValue);

            return currentValue;
        }

        public static float GetGamePadAxis(string axis, int activeGamePad)
        {
            GamePadAxesSettings.TryGetValue(axis, out GamePadAxis gpAxis);
            return (float)Math.Round(GamePad.GetState(activeGamePad).GetAxis(gpAxis), 2);
        }
    }


}
