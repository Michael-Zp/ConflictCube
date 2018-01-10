using ConflictCube.ComponentBased.Components;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased.View
{
    public class Camera
    {
        public Transform Transform;
        public GameObject RootGameObject;
        public FBO FBO;
        public Transform RenderTarget;
        public bool IsUiCamera;

        public Camera(Transform transform, GameObject rootGameObject, int windowWidth, int windowHeight, Transform renderTarget, bool isUiCamera) 
            : this(transform, rootGameObject, new FBO(Texture2dGL.Create(windowWidth, windowHeight, OpenTK.Graphics.OpenGL4.PixelInternalFormat.Rgba)), renderTarget, isUiCamera)
        {}

        public Camera(Transform transform, GameObject rootGameObject, FBO fBO, Transform renderTarget, bool isUiCamera)
        {
            Transform = transform;
            RootGameObject = rootGameObject;
            FBO = fBO;
            RenderTarget = renderTarget;
            IsUiCamera = isUiCamera;
        }
    }
}
