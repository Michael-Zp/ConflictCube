using ConflictCube.ComponentBased.Controller;

namespace ConflictCube.ComponentBased
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Debug options

            DebugGame.BreakDownFloors = false;
            DebugGame.CanLoose = false;
            DebugGame.Player1PrintPosition = false;
            DebugGame.Player2PrintPosition = false;
            DebugGame.PlayerPrintCollisionTypes = false;
            DebugGame.NoClip = false;
            DebugGame.DrawBoxColliderCollisions = false;
            DebugGame.CanDie = false;
            DebugGame.PrintUseFieldPositionOrangePlayer = false;
            DebugGame.DebugDrawUseField = false;
            DebugGame.ShowBoundaries = false;

            //----

            MyWindow window = new MyWindow(1024, 512);
            GameView view = new GameView(window);
            GameState state = new GameState();
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
