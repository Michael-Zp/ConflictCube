using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Controller;
using ConflictCube.ComponentBased.Model.Components.Objects;

namespace ConflictCube.ComponentBased
{
    public class GameState
    {
        public Game Game { get; set; }
        public Scene ActiveScene;

        private int WindowWidth;
        private int WindowHeight;
        
        public GameState(int windowWidth, int windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            BuildMenu();
        }
        
        public void BuildMenu()
        {
            Game = new Game("Game", new Transform(0, 0, 1, 1));

            ActiveScene = SceneBuilder.BuildMenu(this, WindowWidth, WindowHeight);

            Game.AddChild(ActiveScene.RootGameObject);
        }

        public void BuildScene(string level)
        {
            Game = new Game("Game", new Transform(0, 0, 1, 1));

            ActiveScene = SceneBuilder.BuildLevel(this, level, new Transform(), WindowWidth, WindowHeight);

            Game.AddChild(ActiveScene.RootGameObject);
        }

        public void UpdateAll()
        {
            //Axes
            //GameView.DrawDebug(new Transform(0, 0, 1f, .002f), Color.Red);
            //GameView.DrawDebug(new Transform(0, 0, .001f, 1f), Color.Blue);
            
            Game.UpdateAll();
        }

        
        public ViewModel GetViewModel()
        {
            return new ViewModel(this);
        }
    }
}
