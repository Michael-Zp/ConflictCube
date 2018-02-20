using OpenTK;
using Zenseless.OpenGL;
using System;

namespace Engine.Components
{
    public class Camera
    {
        public Transform Transform;
        public GameObject RootGameObject;
        public FBO FBO;
        public Transform RenderTarget;
        public readonly Vector2 OriginalRenderTargetSize;
        public bool IsUiCamera;

        public Camera(Transform transform, GameObject rootGameObject, int windowWidth, int windowHeight, Transform renderTarget, bool isUiCamera) 
            : this(transform, rootGameObject, new FBO(Texture2dGL.Create(windowWidth, windowHeight, OpenTK.Graphics.OpenGL4.PixelInternalFormat.Rgba)), renderTarget, isUiCamera)
        {}


        protected Camera(Transform transform, GameObject rootGameObject, FBO fBO, Transform renderTarget, bool isUiCamera, Vector2 originalRenderTargetSize)
            : this(transform, rootGameObject, fBO, renderTarget, isUiCamera)
        {
            OriginalRenderTargetSize = originalRenderTargetSize;
        }


        public Camera(Transform transform, GameObject rootGameObject, FBO fBO, Transform renderTarget, bool isUiCamera)
        {
            Transform = transform;
            RootGameObject = rootGameObject;
            FBO = fBO;
            RenderTarget = renderTarget;

            if(RenderTarget == null)
            {
                throw new ArgumentNullException("renderTarget is null");
            }

            OriginalRenderTargetSize = RenderTarget.GetSize(WorldRelation.Global);
            IsUiCamera = isUiCamera;
        }
    }
}
