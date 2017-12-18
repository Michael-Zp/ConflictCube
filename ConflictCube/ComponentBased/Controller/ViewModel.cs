using ConflictCube.ComponentBased.View;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Controller
{
    public class ViewModel
    {
        public List<Camera> Cameras = new List<Camera>();

        public ViewModel(GameState state)
        {
            Cameras.Add(state.Player1UICamera);
            Cameras.Add(state.Player2UICamera);
            Cameras.Add(state.Player1Camera);
            Cameras.Add(state.Player2Camera);
        }
    }
}
