using OpenTK;
using System;
using Zenseless.Geometry;

namespace Engine.Components
{
    public class Transform : Component, IEquatable<Transform>
    {
        private Matrix3 TransformMatrix;

        private Vector2 Position {
            get {
                return new Vector2(TransformMatrix.M13, TransformMatrix.M23);
            }
            set {
                TransformMatrix.M13 = value.X;
                TransformMatrix.M23 = value.Y;
            }
        }

        public Vector2 GetPosition(WorldRelation relation)
        {
            if(relation == WorldRelation.Local)
            {
                return Position;
            }
            else
            {
                return TransformToGlobal().Position;
            }
        }

        public void SetPosition(Vector2 position, WorldRelation relation)
        {
            if(relation == WorldRelation.Local)
            {
                Position = position;
            }
            else
            {
                Position = TransformToLocal(new Transform(position.X, position.Y, 1, 1)).Position;
            }
        }

        public void AddToPosition(Vector2 position, WorldRelation relation)
        {
            if(relation == WorldRelation.Local)
            {
                Position += position;
            }
            else
            {
                Position += TransformToLocal(new Transform(position.X, position.Y, 1, 1, 0)).Position;
            }
        }

        private Vector2 Size {
            get {
                return new Vector2(TransformMatrix.M11, TransformMatrix.M22);
            }
            set {
                TransformMatrix.M11 = value.X;
                TransformMatrix.M22 = value.Y;
            }
        }

        public Vector2 GetSize(WorldRelation relation)
        {
            if (relation == WorldRelation.Local)
            {
                return Size;
            }
            else
            {
                return TransformToGlobal().Size;
            }
        }

        public void SetSize(Vector2 size, WorldRelation relation)
        {
            if(relation == WorldRelation.Local)
            {
                Size = size;
            }
            else
            {
                Size = TransformToLocal(new Transform(0, 0, size.X, size.Y)).Size;
            }
        }


        private float Rotation { get; set; }


        /// <summary>
        /// Get rotation in degree;
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public float GetRotation(WorldRelation relation)
        {
            if (relation == WorldRelation.Local)
            {
                return Rotation;
            }
            else
            {
                return TransformToGlobal().Rotation;
            }
        }

        /// <summary>
        /// Only local rotation can be set. Rotation is set in degree.
        /// </summary>
        /// <param name="rotation"></param>
        public void SetRotation(float rotation, WorldRelation relation)
        {
            if (relation == WorldRelation.Local)
            {
                Rotation = rotation;
            }
            else
            {
                Rotation = 0;
                Rotation = rotation - GetGlobalRotation();
            }
        }

        public float GetMinX(WorldRelation relation)
        {
            if(relation == WorldRelation.Local)
            {
                return Position.X - Size.X;
            }
            else
            {
                Transform globalTransform = TransformToGlobal();
                return globalTransform.Position.X - globalTransform.Size.X;
            }
        }
        public float GetMaxX(WorldRelation relation)
        {
            if (relation == WorldRelation.Local)
            {
                return Position.X + Size.X;
            }
            else
            {
                Transform globalTransform = TransformToGlobal();
                return globalTransform.Position.X + globalTransform.Size.X;
            }
        }

        public float GetMinY(WorldRelation relation)
        {
            if (relation == WorldRelation.Local)
            {
                return Position.Y - Size.Y;
            }
            else
            {
                Transform globalTransform = TransformToGlobal();
                return globalTransform.Position.Y - globalTransform.Size.Y;
            }
        }
        public float GetMaxY(WorldRelation relation)
        {
            if (relation == WorldRelation.Local)
            {
                return Position.Y + Size.Y;
            }
            else
            {
                Transform globalTransform = TransformToGlobal();
                return globalTransform.Position.Y + globalTransform.Size.Y;
            }
        }

        public Rectangle GetGlobalNotRotatedRectangle()
        {
            Rectangle rect = new Rectangle(GetMinX(WorldRelation.Global), GetMaxX(WorldRelation.Global), GetMinY(WorldRelation.Global), GetMaxY(WorldRelation.Global));
            return rect;
        }

        public Rectangle GetGlobalRotatedRectangle()
        {
            Rectangle rect = new Rectangle(GetMinX(WorldRelation.Global), GetMaxX(WorldRelation.Global), GetMinY(WorldRelation.Global), GetMaxY(WorldRelation.Global));
            rect = rect.Rotate(Rotation, RotationMode.Degree);
            return rect;
        }

        public Vector2 Forward {
            get {
                Vector2 forward = Vector2.UnitY;
                Matrix2 rotation = Matrix2.CreateRotation(Zenseless.Geometry.MathHelper.DegreesToRadians(GetRotation(WorldRelation.Global)));
                forward = forward.ApplyMatrix2(rotation);
                return forward.Normalized();
            }
        }


        public Transform(Matrix3 matrix, float rotation = 0)
        {
            TransformMatrix = matrix;
            Rotation = rotation;
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
        public Transform(float centerX, float centerY, float sizeX, float sizeY, float vector) : this(centerX, centerY, sizeX, sizeY)
        {
            Position = new Vector2(centerX, centerY);
            Size = new Vector2(sizeX, sizeY);
            TransformMatrix.M33 = vector;
        }

        public Matrix3 GetInverseOfTransform()
        {
            return TransformMatrix.Inverted();
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


        public Matrix3 GetTransformMatrixToGlobal()
        {
            if(Owner == null)
            {
                return TransformMatrix;
            }

            GameObject currentOwner = Owner.Parent;
            Matrix3 currentTransform = TransformMatrix;

            while (currentOwner != null)
            {
                currentTransform = currentOwner.Transform.TransformMatrix * currentTransform;
                currentOwner = currentOwner.Parent;
            }

            return currentTransform;
        }

        public float GetGlobalRotation()
        {
            if(Owner == null)
            {
                return Rotation;
            }

            GameObject currentOwner = Owner.Parent;
            float currentRotation = Rotation;

            while(currentOwner != null)
            {
                currentRotation += currentOwner.Transform.Rotation;
                currentOwner = currentOwner.Parent;
            }

            return currentRotation;
        }

        public Matrix3 GetTransformMatrixToLocal()
        {
            if (Owner == null)
            {
                return new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            }

            GameObject currentOwner = Owner.Parent;
            Matrix3 currentTransform = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            while (currentOwner != null)
            {
                currentTransform = currentOwner.Transform.TransformMatrix * currentTransform;
                currentOwner = currentOwner.Parent;
            }

            return currentTransform.Inverted();
        }

        public Transform TransformToGlobal()
        {
            return TransformToGlobal(new Transform(0, 0, 1, 1));
        }

        public Transform TransformToGlobal(Transform transform)
        {
            Transform tempTrans = new Transform(GetTransformMatrixToGlobal() * transform.TransformMatrix, GetGlobalRotation());
            return tempTrans;
        }

        /*
         * Game * P1A * Scene * Floor * Local = Global
         * Local = (((Floor^-1 * Scene^-1) * P1A^-1) * Game^-1) * Global
         */

        public Transform TransformToLocal(Transform transformToLocal)
        {
            Transform newTransform = (Transform)Clone();
            Transform tempTransform = new Transform(GetTransformMatrixToLocal() * transformToLocal.TransformMatrix, Rotation);
            newTransform.TransformMatrix = tempTransform.TransformMatrix;

            return newTransform;
        }


        public Transform TransformToSpace(Transform space)
        {
            Transform newTransform = new Transform(0, 0, 1, 1)
            {
                TransformMatrix = space.TransformMatrix * TransformMatrix
            };

            return newTransform;
        }

        public override Component Clone()
        {
            Transform newTransform = (Transform)base.Clone();

            newTransform.TransformMatrix = new Matrix3(TransformMatrix.Row0, TransformMatrix.Row1, TransformMatrix.Row2);

            return newTransform;
        }


        /// <summary>
        /// Checks if some transform intersects with another transform.
        /// Includes an epsilon, because when transforming from local to global or vice versa float inaccuarcy can lead to errors.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(Transform other)
        {
            float epsilon = 1e-5f;
            bool noXintersect = (GetMaxX(WorldRelation.Global) - epsilon <= other.GetMinX(WorldRelation.Global)) || (GetMinX(WorldRelation.Global) + epsilon >= other.GetMaxX(WorldRelation.Global));
            bool noYintersect = (GetMaxY(WorldRelation.Global) - epsilon <= other.GetMinY(WorldRelation.Global)) || (GetMinY(WorldRelation.Global) + epsilon >= other.GetMaxY(WorldRelation.Global));
            return !(noXintersect || noYintersect);
        }
        

        public void MoveRelative(Vector2 movement)
        {
            Vector2 globalXMovement = TransformToGlobal(new Transform(movement.X, 0, 0, 0, 0)).Position;
            Vector2 globalYMovement = TransformToGlobal(new Transform(0, movement.Y, 0, 0, 0)).Position;

            AddToPosition(globalXMovement, WorldRelation.Global);
            Collider collider = Owner?.GetComponent<Collider>();

            if (collider != null)
            {
                collider.CheckCollisions(globalXMovement);
            }


            AddToPosition(globalYMovement, WorldRelation.Global);

            if (collider != null)
            {
                collider.CheckCollisions(globalYMovement);
            }
        }


        /// <summary>
        /// Checks if the TransformMatrix of the transforms are the same. The Owner of the Transform is not include in the equals method.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Transform other)
        {
            return (TransformMatrix == other.TransformMatrix);
        }

        public static Transform operator *(Transform transform, Matrix3 matrix)
        {
            return new Transform(matrix * transform.TransformMatrix, transform.GetGlobalRotation());
        }

        public static Transform operator *(Transform transform, Transform other)
        {
            return new Transform(transform.TransformMatrix * other.TransformMatrix, transform.GetGlobalRotation() + other.GetGlobalRotation());
        }
    }


}
