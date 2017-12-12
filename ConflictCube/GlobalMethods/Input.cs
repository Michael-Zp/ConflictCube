﻿using System.Collections.Generic;
using OpenTK.Input;
using System;

namespace ConflictCube.ComponentBased
{
    public enum InputAxis
    {
        Horizontal,
        Vertical
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
        public static Dictionary<Key, InputKey> KeyboardSettings = new Dictionary<Key, InputKey>();
        public static Dictionary<InputAxis, float> Axes = new Dictionary<InputAxis, float>();
        public static Dictionary<InputAxis, AxisData> AxesSettings = new Dictionary<InputAxis, AxisData>();

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
            KeyboardSettings.Add(Key.LShift, InputKey.PlayerOneSprint);


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


            //Axes
            AxesSettings.Add(InputAxis.Horizontal, new AxisData(InputKey.PlayerOneMoveRight, InputKey.PlayerOneMoveLeft));
            AxesSettings.Add(InputAxis.Vertical, new AxisData(InputKey.PlayerOneMoveUp, InputKey.PlayerOneMoveDown));

            Axes.Add(InputAxis.Horizontal, 0f);
            Axes.Add(InputAxis.Vertical, 0f);
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
                else if(!keyIsPressed && lastIsKeyPressed)
                {
                    SetInput(input, ButtonIsPressed, false);
                    SetInput(input, ButtonWasReleased, true);
                }
                else if (!keyIsPressed && !lastIsKeyPressed)
                {
                    SetInput(input, ButtonWasReleased, false);
                }
            }

            UpdateAxes();

            LastState = Keyboard.GetState();
        }

        private static void UpdateAxes()
        {
            UpdateAxis(InputAxis.Horizontal);
            UpdateAxis(InputAxis.Vertical);
        }

        private static void UpdateAxis(InputAxis axis)
        {
            AxesSettings.TryGetValue(axis, out AxisData data);

            bool foundPositiveKey = false, foundNegativeKey = false;
            Key positveKey = Key.A, negativeKey = Key.B; //Keys have to be initialized to be used later. If they are not found in the axis data there is an early exist, so Key.A and Key.B are only spaceholders.

            foreach (Key key in KeyboardSettings.Keys)
            {
                KeyboardSettings.TryGetValue(key, out InputKey inputKey);

                if (inputKey == data.PositiveKey)
                {
                    foundPositiveKey = true;
                    positveKey = key;
                    if (foundNegativeKey)
                        break;
                }

                if (inputKey == data.NegativeKey)
                {
                    foundNegativeKey = true;
                    negativeKey = key;
                    if (foundPositiveKey)
                        break;
                }
            }

            if (!foundPositiveKey || !foundNegativeKey)
            {
                return;
            }

            Axes.TryGetValue(axis, out float currentValue);

            bool positivePressed = Keyboard.GetState().IsKeyDown(positveKey);
            bool negativePressed = Keyboard.GetState().IsKeyDown(negativeKey);

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

            Console.WriteLine(currentValue);

            Axes.Remove(axis);
            Axes.Add(axis, currentValue);
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

        public static float GetAxis(InputAxis axis)
        {
            Axes.TryGetValue(axis, out float currentValue);

            return currentValue;
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