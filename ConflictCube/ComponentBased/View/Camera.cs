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

        public Camera(Transform transform, GameObject rootGameObject, int windowWidth, int windowHeight, Transform renderTarget) 
            : this(transform, rootGameObject, new FBO(Texture2dGL.Create(windowWidth, windowHeight, OpenTK.Graphics.OpenGL4.PixelInternalFormat.Rgba)), renderTarget)
        {}

        public Camera(Transform transform, GameObject rootGameObject, FBO fBO, Transform renderTarget)
        {
            Transform = transform;
            RootGameObject = rootGameObject;
            FBO = fBO;
            RenderTarget = renderTarget;
        }
    }
}
