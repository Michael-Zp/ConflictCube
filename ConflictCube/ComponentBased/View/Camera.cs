using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.View
{
    public class Camera
    {
        public Transform Transform;
        public GameObject RootGameObject;

        public Camera(Transform transform, GameObject rootGameObject)
        {
            Transform = transform;
            RootGameObject = rootGameObject;
        }
    }
}
