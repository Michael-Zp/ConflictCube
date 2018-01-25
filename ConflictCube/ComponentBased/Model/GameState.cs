using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Controller;
using ConflictCube.ComponentBased.Model.Components.Objects;

namespace ConflictCube.ComponentBased
{
    public class GameState : IBuildScene, IBuildMenu
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
            Game = new Game("ActiveScene", new Transform(0, 0, 1, 1), null);

            ActiveScene = SceneBuilder.BuildMenu(this, WindowWidth, WindowHeight, Game);
        }

        public void BuildScene(string level)
        {
            Game = new Game("ActiveScene", new Transform(0, 0, 1, 1), null);

            ActiveScene = SceneBuilder.BuildLevel(this, level, new Transform(), WindowWidth, WindowHeight, Game);
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
