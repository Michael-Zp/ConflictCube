using OpenTK.Input;

namespace ConflictCube
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MyWindow window = new MyWindow();
            GameView view = new GameView(window);
            GameState state = new GameState(view);
            GameController controller = new GameController(state);
           

            controller.LoadLevel(0);

            while(window.WaitForNextFrame())
            {
                controller.UpdateState();
                state.NextFrame(window.TimeDiff());
                state.UpdateView();
                view.Render();
            }
        }
    }
}
