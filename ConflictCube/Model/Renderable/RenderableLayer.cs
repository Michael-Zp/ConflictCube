using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictCube.Model.Renderable
{
    public enum RenderLayerType
    {
        Floor
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
