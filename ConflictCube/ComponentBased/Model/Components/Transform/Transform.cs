using OpenTK;
using Zenseless.Geometry;

namespace ConflictCube.ComponentBased.Components
{
    public class Transform : Component
    {
        private Matrix3 TransformMatrix;

        public Vector2 Position {
            get {
                return new Vector2(TransformMatrix.M13, TransformMatrix.M23);
            }
            set {
                TransformMatrix.M13 = value.X;
                TransformMatrix.M23 = value.Y;
            }
        }

        public Vector2 Size {
            get {
                return new Vector2(TransformMatrix.M11, TransformMatrix.M22);
            }
            set {
                TransformMatrix.M11 = value.X;
                TransformMatrix.M22 = value.Y;
                Collider collider = Owner?.GetComponent<Collider>();

                if (collider != null)
                {
                    collider.CheckCollisions(value);
                }
            }
        }

        public float MinX { get { return Position.X - Size.X; } }
        public float MaxX { get { return Position.X + Size.X; } }

        public float MinY { get { return Position.Y - Size.Y; } }
        public float MaxY { get { return Position.Y + Size.Y; } }

        public Transform(Matrix3 matrix)
        {
            TransformMatrix = matrix;
        }

        public Transform() : this(0, 0, 1, 1)
        { }

        public Transform(Box2D box) : this(box.CenterX, box.CenterY, box.SizeX, box.SizeY)
        { }

        public Transform(float centerX, float centerY, float sizeX, float sizeY)
        {
            Position = new Vector2(centerX, centerY);
            Size = new Vector2(sizeX, sizeY);
            TransformMatrix.M33 = 1;
        }

        public override void SetOwner(GameObject owner)
        {
            base.SetOwner(owner);
        }

        public Transform TransformToParent()
        {
            Matrix3 currentTransform = TransformMatrix;
            if (Owner.Parent != null)
            {
                currentTransform = Owner.Parent.Transform.TransformMatrix * TransformMatrix;
            }

            Transform newTransform = (Transform)Clone();
            newTransform.TransformMatrix = currentTransform;

            return newTransform;
        }

        public Transform TransformToLocal(Transform transformToLocal)
        {
            GameObject currentOwner = Owner.Parent;
            Matrix3 currentTransform = transformToLocal.TransformMatrix;

            while (currentOwner != null)
            {
                currentTransform = currentOwner.Transform.TransformMatrix.Inverted() * currentTransform;
                currentOwner = currentOwner.Parent;
            }

            Transform newTransform = (Transform)Clone();
            newTransform.TransformMatrix = currentTransform;

            return newTransform;
        }

        public Transform TransformToGlobal()
        {
            return TransformToGlobal(this);
        }

        public Transform TransformToGlobal(Transform transform)
        {
            if(Owner == null)
            {
                return transform;
            }
            GameObject currentOwner = Owner.Parent;
            Matrix3 currentTransform = transform.TransformMatrix;

            while (currentOwner != null)
            {
                currentTransform = currentOwner.Transform.TransformMatrix * currentTransform;
                currentOwner = currentOwner.Parent;
            }

            Transform newTransform = (Transform)Clone();
            newTransform.TransformMatrix = currentTransform;
            
            return newTransform;
        }

        public Transform TransformToSpace(Transform space)
        {
            Transform newTransform = (Transform)Clone();
            newTransform.TransformMatrix = space.TransformMatrix * TransformMatrix;

            return newTransform;
        }

        public override Component Clone()
        {
            Transform newTransform = (Transform)base.Clone();

            newTransform.TransformMatrix = new Matrix3(TransformMatrix.Row0, TransformMatrix.Row1, TransformMatrix.Row2);

            return newTransform;
        }

        public bool Intersects(Transform other)
        {
            bool noXintersect = (MaxX <= other.MinX) || (MinX >= other.MaxX);
            bool noYintersect = (MaxY <= other.MinY) || (MinY >= other.MaxY);
            return !(noXintersect || noYintersect);
        }

        public void MoveRelative(Vector2 movement)
        {
            Position += movement;
            Collider collider = Owner?.GetComponent<Collider>();

            if (collider != null)
            {
                collider.CheckCollisions(movement);
            }
        }

        public static Transform operator *(Transform transform, Matrix3 matrix)
        {
            return new Transform(matrix * transform.TransformMatrix);
        }

        public static Transform operator *(Transform transform, Transform other)
        {
            return new Transform(transform.TransformMatrix * other.TransformMatrix);
        }
    }
}
