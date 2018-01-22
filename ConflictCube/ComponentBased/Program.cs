using ConflictCube.ComponentBased.Controller;

namespace ConflictCube.ComponentBased
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Debug options

            DebugGame.BreakDownFloors = false;
            DebugGame.CanLoose = true;
            DebugGame.Player1PrintPosition = false;
            DebugGame.Player2PrintPosition = false;
            DebugGame.PlayerPrintCollisionTypes = false;
            DebugGame.NoClip = false;
            DebugGame.DrawBoxColliderCollisions = false;
            DebugGame.CanDie = true;
            DebugGame.PrintUseFieldPositionOrangePlayer = false;
            DebugGame.DebugDrawUseField = false;
            DebugGame.ShowBoundaries = false;
            DebugGame.PrintFPS = true;

            //End debug options----

            MyWindow window = new MyWindow(512, 512);
            GameView view = new GameView(window);
            GameState state = new GameState(window.Width, window.Height);
            GameController controller = new GameController();



            while(window.WaitForNextFrame())
            {
                Time.Time.CurrentTime = window.GetTime();

                controller.UpdateInputs();
                state.UpdateAll();
                view.Render(state.GetViewModel());
            }
        }
    }
}
