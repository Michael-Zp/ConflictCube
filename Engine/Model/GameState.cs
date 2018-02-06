using Engine.Components;
using Engine.Controler;
using Engine.ModelView;
using Engine.Scenes;
using Engine.View;
using System.Drawing;

namespace Engine.Model
{
    public class GameState
    {
        public Scene ActiveScene;

        private int WindowWidth;
        private int WindowHeight;
        
        public GameState(int windowWidth, int windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }
        
        public void UpdateGameState()
        {
            //Axes
            if(Engine.Debug.DebugEngine.DrawXandYAxis)
            {
                GameView.DrawDebug(new Transform(0, 0, 1f, .002f), Color.Red);
                GameView.DrawDebug(new Transform(0, 0, .001f, 1f), Color.Blue);
            }
            
            ActiveScene.RootGameObject.UpdateAll();
            GameObject.DestroyGameObjects();
        }

        public void ClearGameObjectTree()
        {
            if(ActiveScene.RootGameObject != null)
                GameObject.Destroy(ActiveScene.RootGameObject);
        }
        
        public ViewModel GetViewModel()
        {
            return new ViewModel(this);
        }
    }
}
