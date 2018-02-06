using Engine.Components;
using Engine.Model;
using System.Collections.Generic;

namespace Engine.ModelView
{
    public class ViewModel
    {
        public List<ViewModelCamera> Cameras = new List<ViewModelCamera>();
        
        public ViewModel(GameState state)
        {
            foreach(Camera camera in state.ActiveScene.Cameras)
            {
                Cameras.Add(new ViewModelCamera(camera));
            }
        }
    }
}
