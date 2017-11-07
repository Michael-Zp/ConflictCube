using ConflictCube.Model;
using ConflictCube.Controller;

namespace ConflictCube
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MyWindow window = new MyWindow();
            GameView view = new GameView(window);
            GameState state = new GameState();
            GameController controller = new GameController();
           

            while(window.WaitForNextFrame())
            {
                var inputs = controller.GetInputs();
                state.Update(inputs, window.TimeDiff());
                var viewModel = state.GetViewModel();
                view.Render(viewModel);
            }
        }
    }
}
