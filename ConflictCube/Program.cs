using Engine.Inputs;
using Engine.Debug;
using OpenTK.Input;
using ConflictCube.Debug;
using System.Collections.Generic;
using Engine.Components;
using Engine;
using ConflictCube.SceneBuilders;

namespace ConflictCube
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Debug game options
            
            DebugGame.CanLoose = true;
            DebugGame.Player1PrintPosition = false;
            DebugGame.Player2PrintPosition = false;
            DebugGame.PlayerPrintCollisionTypes = false;
            DebugGame.NoClip = false;
            DebugGame.CanDie = true;
            DebugGame.PrintUseFieldPositionOrangePlayer = false;
            DebugGame.DebugDrawUseField = false;
            DebugGame.ShowBoundaries = false;

            //Debug engine options

            DebugEngine.DrawBoxColliderCollisions = false;
            DebugEngine.DrawXandYAxis = false;
            DebugEngine.PrintFPS = false;

            InitalizeInputs();
            InitalizeCollisionLayers();

            GameEngine engine = new GameEngine();
            SceneBuilder sceneBuilder = new SceneBuilder(1024, 1024, engine.SceneManager);

            sceneBuilder.BuildMenu();

            engine.RunGameLoop();
        }


        private static void InitalizeInputs()
        {
            //GamePad and Keyboard - General

            Input.KeyboardSettings.Add("ExitGame", Key.Escape);
            Input.GamePadSettings.Add("ExitGame", GamePadButton.RightStick);


            //GamePad - Player 1

            Input.GamePadSettings.Add("PlayerOneSprint", GamePadButton.RightShoulder);
            Input.GamePadSettings.Add("PlayerOneUse", GamePadButton.X);
            Input.GamePadSettings.Add("PlayerOneSwitchPositionX", GamePadButton.Y);
            Input.GamePadSettings.Add("PlayerOneSwitchPositionXY", GamePadButton.A);
            Input.GamePadSettings.Add("PlayerOneSwitchPositionY", GamePadButton.B);

            //Player 2

            Input.GamePadSettings.Add("PlayerTwoSprint", GamePadButton.RightShoulder);
            Input.GamePadSettings.Add("PlayerTwoPlayerTwoUse", GamePadButton.X);
            Input.GamePadSettings.Add("PlayerTwoPlayerTwoSwitchPositionsX", GamePadButton.Y);
            Input.GamePadSettings.Add("PlayerTwoPlayerTwoSwitchPositionsXY", GamePadButton.A);
            Input.GamePadSettings.Add("PlayerTwoPlayerTwoSwitchPositionsY", GamePadButton.B);


            //Gamepad - Axes - Player 1

            Input.GamePadAxesSettings.Add("PlayerAxisPlayer1Horizontal", GamePadAxis.LeftPadX);
            Input.GamePadAxesSettings.Add("PlayerAxisPlayer1Vertical", GamePadAxis.LeftPadY);

            //Player 2

            Input.GamePadAxesSettings.Add("PlayerAxisPlayer2Horizontal", GamePadAxis.LeftPadX);
            Input.GamePadAxesSettings.Add("PlayerAxisPlayer2Vertical", GamePadAxis.LeftPadY);


            //Keyboard - Player 1
            Input.KeyboardSettings.Add("PlayerTwoPlayerOneMoveLeft", Key.A);
            Input.KeyboardSettings.Add("PlayerTwoPlayerOneMoveRight", Key.D);
            Input.KeyboardSettings.Add("PlayerTwoPlayerOneMoveUp", Key.W);
            Input.KeyboardSettings.Add("PlayerTwoPlayerOneMoveDown", Key.S);
            Input.KeyboardSettings.Add("PlayerOneSprint", Key.LShift);
            Input.KeyboardSettings.Add("PlayerOneUse", Key.E);
            Input.KeyboardSettings.Add("PlayerOneSwitchPositionY", Key.R);
            Input.KeyboardSettings.Add("PlayerOneSwitchPositionXY", Key.Q);
            Input.KeyboardSettings.Add("PlayerOneSwitchPositionX", Key.F);
            Input.KeyboardSettings.Add("PlayerTwoZoom", Key.Space);




            //Player 2
            Input.KeyboardSettings.Add("PlayerTwoPlayerTwoMoveLeft", Key.J);
            Input.KeyboardSettings.Add("PlayerTwoPlayerTwoMoveRight", Key.L);
            Input.KeyboardSettings.Add("PlayerTwoPlayerTwoMoveUp", Key.I);
            Input.KeyboardSettings.Add("PlayerTwoPlayerTwoMoveDown", Key.K);
            Input.KeyboardSettings.Add("PlayerTwoSprint", Key.RControl);
            Input.KeyboardSettings.Add("PlayerTwoPlayerTwoUse", Key.O);
            Input.KeyboardSettings.Add("PlayerTwoPlayerTwoSwitchPositionsY", Key.Y); //Uses QWERTY layout
            Input.KeyboardSettings.Add("PlayerTwoPlayerTwoSwitchPositionsXY", Key.U);
            Input.KeyboardSettings.Add("PlayerTwoPlayerTwoSwitchPositionsX", Key.H);


            //Axes
            Input.AxesSettings.Add("PlayerAxisPlayer1Horizontal", new AxisData("PlayerTwoPlayerOneMoveRight", "PlayerTwoPlayerOneMoveLeft"));
            Input.AxesSettings.Add("PlayerAxisPlayer1Vertical", new AxisData("PlayerTwoPlayerOneMoveUp", "PlayerTwoPlayerOneMoveDown"));
            Input.AxesSettings.Add("PlayerAxisPlayer2Horizontal", new AxisData("PlayerTwoPlayerTwoMoveRight", "PlayerTwoPlayerTwoMoveLeft"));
            Input.AxesSettings.Add("PlayerAxisPlayer2Vertical", new AxisData("PlayerTwoPlayerTwoMoveUp", "PlayerTwoPlayerTwoMoveDown"));


            Input.Axes.Add("PlayerAxisPlayer1Horizontal", 0f);
            Input.Axes.Add("PlayerAxisPlayer1Vertical", 0f);
            Input.Axes.Add("PlayerAxisPlayer2Horizontal", 0f);
            Input.Axes.Add("PlayerAxisPlayer2Vertical", 0f);
        }


        private static void InitalizeCollisionLayers()
        {
            CollisionLayers.AddLayer("Default", new List<string> { "Default", "Orange", "Blue" });
            CollisionLayers.AddLayer("Orange", new List<string> { "Default", "Blue" });
            CollisionLayers.AddLayer("Blue", new List<string> { "Default", "Orange" });
        }
    }
}
