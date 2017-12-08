using ConflictCube.ComponentBased.Controller;

namespace ConflictCube.ComponentBased
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
