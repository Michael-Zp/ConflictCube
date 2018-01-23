using ConflictCube.ComponentBased.View;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Controller
{
    public class ViewModel
    {
        public List<Camera> Cameras = new List<Camera>();
        
        public ViewModel(GameState state)
        {
            foreach(Camera camera in state.ActiveScene.Cameras)
            {
                Cameras.Add(camera);
            }
        }
    }
}
