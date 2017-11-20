using OpenTK;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ConflictCube.Model.Renderable
{
    public enum RenderLayerType
    {
        Floor,
        Player,
        UI,
        ThrowUseIndicator
    }

    public class RenderableLayer
    {
        public List<RenderableLayer> SubLayers { protected get; set; }
        public List<RenderableObject> ObjectsToRender { protected get; set; }
        protected Box2D _AreaOfLayer;
        public Box2D AreaOfLayer {
            get {
                return _AreaOfLayer;
            }
            set {
                _AreaOfLayer = value;
                ScaleMatrix = GenerateScaleMatrix();
            }
        }
        public Matrix3 ScaleMatrix { get; protected set; }

        public RenderableLayer(List<RenderableObject> objectsToRender, List<RenderableLayer> subLayers, Box2D areaOfLayer)
        {
            ObjectsToRender = objectsToRender;
            SubLayers = subLayers;
            AreaOfLayer = areaOfLayer;
        }

        public List<RenderableObject> GetRenderableObjects()
        {
            List<RenderableObject> clonedList = new List<RenderableObject>();

            foreach(RenderableObject obj in ObjectsToRender)
            {
                clonedList.Add(obj.Clone());
            }
            
            foreach (RenderableLayer layer in SubLayers)
            {
                clonedList.AddRange(layer.GetRenderableObjects());
            }

            foreach (RenderableObject clone in clonedList)
            {
                //Scale
                Vector2 newSize = TransformSizeToParent(clone.Box.SizeX, clone.Box.SizeY);
                clone.Box.SizeX = newSize.X;
                clone.Box.SizeY = newSize.Y;

                //Transform
                Vector2 newPosition = TransformPointToParent(clone.Box.MinX, clone.Box.MinY);
                clone.Box.MinX = newPosition.X;
                clone.Box.MinY = newPosition.Y;
            }

            return clonedList;
        }
        
        protected Matrix3 GenerateScaleMatrix()
        {
            float sizeRatioRows    = _AreaOfLayer.SizeX / 2;
            float sizeRatioColumns = _AreaOfLayer.SizeY / 2;

            return Matrix3.CreateScale(sizeRatioRows, sizeRatioColumns, 1);
        }

        public Vector2 TransformSizeToParent(Vector2 size)
        {
            return TransformSizeToParent(size.X, size.Y);
        }

        public Vector2 TransformSizeToParent(float sizeX, float sizeY)
        {
            Vector3 newSize = Vector3.Transform(new Vector3(sizeX, sizeY, 1), ScaleMatrix);

            return newSize.Xy;
        }

        public Vector2 TransformPointToParent(Vector2 point)
        {
            return TransformPointToParent(point.X, point.Y);
        }

        public Vector2 TransformPointToParent(float posX, float posY)
        {
            posX = posX * ScaleMatrix.Column0[0] + _AreaOfLayer.CenterX;
            posY = posY * ScaleMatrix.Column1[1] + _AreaOfLayer.CenterY;

            return new Vector2(posX, posY);
        }

        public Vector2 TransformPointToLocal(Vector2 point)
        {
            return TransformPointToLocal(point.X, point.Y);
        }

        public Vector2 TransformPointToLocal(float posX, float posY)
        {
            posX = (posX - _AreaOfLayer.CenterX) / ScaleMatrix.Column0[0];
            posY = (posY - _AreaOfLayer.CenterY) / ScaleMatrix.Column1[1];

            return new Vector2(posX, posY);
        }

        public void AddRangedObjectsToRender(IEnumerable<RenderableObject> toAdd)
        {
            if(toAdd != null)
            {
                ObjectsToRender.AddRange(toAdd);
            }
        }

        public void AddObjectsToRender(RenderableObject toAdd)
        {
            if (toAdd != null)
            {
                ObjectsToRender.Add(toAdd);
            }
        }   
        
        public List<ICollidable> GetColliders()
        {
            List<ICollidable> colliders = new List<ICollidable>();

            foreach(RenderableObject obj in GetRenderableObjects())
            {
                if(obj is ICollidable)
                {
                    colliders.Add((ICollidable)obj);
                }
            }

            colliders.AddRange(GetAdditionalColliders());

            return colliders;
        }

        virtual protected List<ICollidable> GetAdditionalColliders()
        {
            return new List<ICollidable>();
        }
    }
}