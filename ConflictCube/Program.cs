using ConflictCube.Model;
using ConflictCube.Controller;

namespace ConflictCube
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

                var inputs = controller.GetInputs();
                state.Update(inputs);
                var viewModel = state.GetViewModel();
                view.Render(viewModel);
            }
        }
    }
}
