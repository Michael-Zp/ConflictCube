using Engine.Controler;
using Engine.Model;
using Engine.Scenes;
using Engine.View;

namespace Engine
{
    public class GameEngine
    {
        public readonly SceneManager SceneManager;

        private readonly MyWindow Window;
        private readonly GameView View;
        private readonly GameState State;
        private readonly GameControler GameControler;

        public GameEngine()
        {
            Window = new MyWindow(512, 512);
            View = new GameView(Window);
            State = new GameState(Window.Width, Window.Height);
            GameControler = new GameControler();
            SceneManager = new SceneManager(State);
        }

        public void RunGameLoop()
        {
            while (Window.WaitForNextFrame())
            {
                Time.Time.CurrentTime = Window.GetTime();

                GameControler.UpdateInputs();
                State.UpdateGameState();
                View.Render(State.GetViewModel());
            }
        }
    }
}
