using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.Controller
{
    public class ViewModel
    {
        public GameObject Scene;
        public GameObject UI;

        public ViewModel(GameState state)
        {
            Scene = state.Scene;
            UI = state.PlayerUIs;
        }
    }
}
