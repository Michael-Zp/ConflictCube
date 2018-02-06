using Engine.Components;
using System.Collections.Generic;

namespace Engine.Scenes
{
    public struct Scene
    {
        public readonly GameObject RootGameObject;
        public readonly List<Camera> Cameras;

        public Scene(GameObject rootGameObject, List<Camera> cameras)
        {
            RootGameObject = rootGameObject;
            Cameras = cameras;
        }
    }
}
