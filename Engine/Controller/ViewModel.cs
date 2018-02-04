using Engine.Components;
using Engine.Model;
using System.Collections.Generic;

namespace Engine.Controler
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
