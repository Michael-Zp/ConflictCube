using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.Controller
{
    public class ViewModel
    {
        public GameObject Game;

        public ViewModel(GameState state)
        {
            Game = state.Game;
        }
    }
}
