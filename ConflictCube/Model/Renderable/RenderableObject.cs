using Zenseless.Geometry;
using OpenTK;
using ConflictCube.Model.Tiles;
using System;

namespace ConflictCube.Model.Renderable
{
    public abstract class RenderableObject
    {
        private static int CurrentID = 0;
        public int ID { get; private set; }
        private Box2D _Box { get; set; }
        public Box2D Box {
            get {
                return _Box;
            }
            set {
                _Box = value;
                OnBoxChanged();
            }
        }
        public TileType Type { get; set; }

        public RenderableObject(Box2D box, TileType type)
        {
            ID = CurrentID;
            Box = box;
            Type = type;

            CurrentID++;
        }

        public RenderableObject(Vector2 position, Vector2 size, TileType type) : this(new Box2D(position.X, position.Y, size.X, size.Y), type)
        {}

        public virtual RenderableObject Clone()
        {
            RenderableObject clone = (RenderableObject)this.MemberwiseClone();
            clone.Box = new Box2D(Box);

            return clone;
        }

        public abstract void SetPosition(Vector2 pos);

        public void ChangeBox(float minX, float minY, float sizeX, float sizeY)
        {
            Box.MinX = minX;
            Box.MinY = minY;
            Box.SizeX = sizeX;
            Box.SizeY = sizeY;
            OnBoxChanged();
        }

        public abstract void OnBoxChanged();
    }
}
