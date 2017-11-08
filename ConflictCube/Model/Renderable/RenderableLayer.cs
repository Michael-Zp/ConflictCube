using System.Collections.Generic;

namespace ConflictCube.Model.Renderable
{
    public enum RenderLayerType
    {
        Floor,
        Player
    }

    public class RenderableLayer
    {
        public List<RenderableObject> ObjectsToRender { get; private set; }

        public RenderableLayer(List<RenderableObject> objectsToRender)
        {
            ObjectsToRender = objectsToRender;
        }

        public RenderableLayer()
        {
            ObjectsToRender = new List<RenderableObject>();
        }
    }
}
