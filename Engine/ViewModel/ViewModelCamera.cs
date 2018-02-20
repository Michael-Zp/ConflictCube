using Engine.Components;
using OpenTK;

namespace Engine.ModelView
{
    public class ViewModelCamera : Camera
    {
        public readonly IViewModelElement RootViewModelElement;

        public ViewModelCamera(Camera camera) : base(camera.Transform, camera.RootGameObject, camera.FBO, camera.RenderTarget, camera.IsUiCamera, camera.OriginalRenderTargetSize)
        {
            RootViewModelElement = new ViewModelElement(camera.RootGameObject);
        }
    }
}
