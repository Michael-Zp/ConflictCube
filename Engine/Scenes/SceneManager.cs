using Engine.Model;

namespace Engine.Scenes
{
    public class SceneManager : IChangeScene
    {
        private GameState State;

        public SceneManager(GameState state)
        {
            State = state;
        }

        public void SetActiveScene(Scene scene)
        {
            State.ClearGameObjectTree();
            State.ActiveScene = scene;
        }
    }
}
