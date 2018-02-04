using OpenTK.Input;
using System;

namespace ConflictCube.GlobalMethods
{
    public enum GamePadButton
    {
        A,
        B,
        X,
        Y,
        LeftStick,
        RightStick,
        LeftShoulder,
        RightShoulder,
        Back,
        Start,
        None,
        DPadUp,
        DPadDown,
    }

    public enum GamePadAxis
    {
        LeftPadX,
        LeftPadY,
    }

    public static class GamePadStateExtensionMethods
    {
        public static bool IsPressed(this GamePadState state, GamePadButton button)
        {
            switch(button)
            {
                case GamePadButton.A:
                    return state.Buttons.A == ButtonState.Pressed;

                case GamePadButton.B:
                    return state.Buttons.B == ButtonState.Pressed;

                case GamePadButton.X:
                    return state.Buttons.X == ButtonState.Pressed;

                case GamePadButton.Y:
                    return state.Buttons.Y == ButtonState.Pressed;

                case GamePadButton.LeftStick:
                    return state.Buttons.LeftStick == ButtonState.Pressed;

                case GamePadButton.RightStick:
                    return state.Buttons.RightStick == ButtonState.Pressed;

                case GamePadButton.LeftShoulder:
                    return state.Buttons.LeftShoulder == ButtonState.Pressed;

                case GamePadButton.RightShoulder:
                    return state.Buttons.RightShoulder == ButtonState.Pressed;

                case GamePadButton.Back:
                    return state.Buttons.Back == ButtonState.Pressed;

                case GamePadButton.Start:
                    return state.Buttons.Start == ButtonState.Pressed;

                case GamePadButton.DPadUp:
                    return state.DPad.Up == ButtonState.Pressed;

                case GamePadButton.DPadDown:
                    return state.DPad.Down == ButtonState.Pressed;

            }

            throw new System.Exception("No Button found for GamePadButton " + button.ToString());
        }

        public static float GetAxis(this GamePadState state, GamePadAxis axis)
        {
            switch(axis)
            {
                case GamePadAxis.LeftPadX:
                    return state.ThumbSticks.Left.X;

                case GamePadAxis.LeftPadY:
                    return state.ThumbSticks.Left.Y;
            }

            throw new System.Exception("No Axis found for GamePadAxis " + axis.ToString());
        }
    }
}
