using OpenTK;
using Zenseless.Geometry;

namespace ConflictCube.ComponentBased.Components
{
    public class Transform : Component
    {
        private Vector2 _Position;
        public Vector2 Position {
            get {
                return _Position;
            }
            set {
                _Position = value;
                Collider collider = Owner.GetComponent<Collider>();

                if (Owner.GetComponent<Collider>() != null)
                {
                    collider.CheckCollisions(value);
                }
            }
        }

        private Vector2 _Size;
        public Vector2 Size {
            get {
                return _Size;
            }
            set {
                _Size = value;
                Collider collider = Owner.GetComponent<Collider>();

                if (Owner.GetComponent<Collider>() != null)
                {
                    collider.CheckCollisions(value);
                }
            }
        }

        public float MinX { get { return Position.X - Size.X / 2; } }
        public float MaxX { get { return Position.X + Size.X / 2; } }

        public float MinY { get { return Position.Y - Size.Y / 2; } }
        public float MaxY { get { return Position.Y + Size.Y / 2; } }

        public Matrix3 ScaleMatrix;

        public Transform()
        {
            _Position = new Vector2(0, 0);
            _Size = new Vector2(1, 1);
            Initialize();
        }

        public Transform(Box2D box)
        {
            _Position = new Vector2(box.CenterX, box.CenterY);
            _Size = new Vector2(box.SizeX, box.SizeY);
            Initialize();
        }

        public Transform(float centerX, float centerY, float sizeX, float sizeY)
        {
            _Position = new Vector2(centerX, centerY);
            _Size = new Vector2(sizeX, sizeY);
            Initialize();
        }
        
        private void Initialize()
        {
            ScaleMatrix = GenerateScaleMatrix();
        }

        private Matrix3 GenerateScaleMatrix()
        {
            float sizeRatioRows = Size.X / 2;
            float sizeRatioColumns = Size.X / 2;

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
            posX = posX * ScaleMatrix.Column0[0] + Position.X;
            posY = posY * ScaleMatrix.Column1[1] + Position.Y;

            return new Vector2(posX, posY);
        }

        public Vector2 TransformPointToLocal(Vector2 point)
        {
            return TransformPointToLocal(point.X, point.Y);
        }

        public Vector2 TransformPointToLocal(float posX, float posY)
        {
            posX = (posX - Position.X) / ScaleMatrix.Column0[0];
            posY = (posY - Position.Y) / ScaleMatrix.Column1[1];

            return new Vector2(posX, posY);
        }

        public override Component Clone()
        {
            Transform newTransform = (Transform)base.Clone();

            newTransform.Position = new Vector2(_Position.X, _Position.Y);
            newTransform.Size = new Vector2(_Size.X, _Size.Y);

            return newTransform;
        }

        public bool Intersects(Transform other)
        {
            bool noXintersect = (MaxX <= other.MinX) || (MinX >= other.MaxX);
            bool noYintersect = (MaxY <= other.MinY) || (MinY >= other.MaxY);
            return !(noXintersect || noYintersect);
        }
    }
}
