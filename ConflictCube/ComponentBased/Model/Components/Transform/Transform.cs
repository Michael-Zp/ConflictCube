using OpenTK;
using System;
using Zenseless.Geometry;

namespace ConflictCube.ComponentBased.Components
{
    public enum WorldRelation
    {
        Global,
        Local
    }

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
        public Transform(float centerX, float centerY, float sizeX, float sizeY, float vector) : this(centerX, centerY, sizeX, sizeY)
        {
            Position = new Vector2(centerX, centerY);
            Size = new Vector2(sizeX, sizeY);
            TransformMatrix.M33 = vector;
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
            Transform tempTrans =  new Transform(GetTransformMatrixToGlobal() * transform.TransformMatrix);
            return tempTrans;
        }

        /*
         * Game * P1A * Scene * Floor * Local = Global
         * Local = (((Floor^-1 * Scene^-1) * P1A^-1) * Game^-1) * Global
         */

        public Transform TransformToLocal(Transform transformToLocal)
        {
            Transform newTransform = (Transform)Clone();
            Transform tempTransform = new Transform(GetTransformMatrixToLocal() * transformToLocal.TransformMatrix);
            newTransform.TransformMatrix = tempTransform.TransformMatrix;

            return newTransform;
        }


        public Transform TransformToSpace(Transform space)
        {
            Transform newTransform = new Transform(0, 0, 1, 1);
            newTransform.TransformMatrix = space.TransformMatrix * TransformMatrix;

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
            return new Transform(matrix * transform.TransformMatrix);
        }

        public static Transform operator *(Transform transform, Transform other)
        {
            return new Transform(transform.TransformMatrix * other.TransformMatrix);
        }
    }


}
