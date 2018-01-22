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

    public struct Rectangle
    {
        public Vector2 BottomLeft;
        public Vector2 BottomRight;
        public Vector2 TopRight;
        public Vector2 TopLeft;

        public Rectangle(float minX, float maxX, float minY, float maxY)
        {
            BottomLeft = new Vector2(minX, minY);
            BottomRight = new Vector2(maxX, minY);
            TopRight = new Vector2(maxX, maxY);
            TopLeft = new Vector2(minX, maxY);
        }

        public Rectangle(Vector2 bottomLeft, Vector2 bottomRight, Vector2 topRight, Vector2 topLeft)
        {
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            TopRight = topRight;
            TopLeft = topLeft;
        }
    }

    public enum RotationMode
    {
        Degree,
        Radians
    }

    public static class RectangleExtensionMethods
    {
        public static Rectangle Rotate(this Rectangle rect, float angle, RotationMode mode)
        {
            if(angle == 0)
            {
                return rect;
            }

            if(mode == RotationMode.Degree)
            {
                angle = Zenseless.Geometry.MathHelper.DegreesToRadians(angle);
            }

            Matrix2 rotationMatrix = Matrix2.CreateRotation(angle);


            Vector2 offsetToCenterPoints = new Vector2(-rect.BottomLeft.X - (rect.BottomRight.X - rect.BottomLeft.X) / 2, -rect.BottomLeft.Y - (rect.TopLeft.Y - rect.BottomLeft.Y) / 2);

            rect.BottomLeft += offsetToCenterPoints;
            rect.BottomRight += offsetToCenterPoints;
            rect.TopRight += offsetToCenterPoints;
            rect.TopLeft += offsetToCenterPoints;

            rect.BottomLeft  = rect.BottomLeft.ApplyMatrix2(rotationMatrix);
            rect.BottomRight = rect.BottomRight.ApplyMatrix2(rotationMatrix);
            rect.TopRight    = rect.TopRight.ApplyMatrix2(rotationMatrix);
            rect.TopLeft     = rect.TopLeft.ApplyMatrix2(rotationMatrix);

            rect.BottomLeft -= offsetToCenterPoints;
            rect.BottomRight -= offsetToCenterPoints;
            rect.TopRight -= offsetToCenterPoints;
            rect.TopLeft -= offsetToCenterPoints;

            return rect;
        }

        public static Rectangle Scale(this Rectangle rect, Vector2 scale)
        {
            float centerX = rect.BottomLeft.X + (rect.BottomRight.X - rect.BottomLeft.X) / 2;
            float centerY = rect.BottomLeft.Y + (rect.TopLeft.Y - rect.BottomLeft.Y) / 2;

            float sizeX = rect.BottomRight.X - centerX;
            float sizeY = rect.BottomRight.Y - centerY;

            float minX = centerX - sizeX * scale.X;
            float maxX = centerX + sizeX * scale.X;
            float minY = centerY - sizeY * scale.Y;
            float maxY = centerY + sizeY * scale.Y;

            return new Rectangle(minX, maxX, minY, maxY);
        }

        public static Rectangle ApplyTransform(this Rectangle rect, Transform transform)
        {

            Vector2 scale = transform.GetSize(WorldRelation.Global);

            //The rect could be rotated, BottomLeft could be at the TopRight position
            float _minX = Math.Min(rect.BottomLeft.X, rect.TopRight.X);
            float _maxX = Math.Max(rect.BottomLeft.X, rect.TopRight.X);
            float _minY = Math.Min(rect.BottomLeft.Y, rect.TopRight.Y);
            float _maxY = Math.Max(rect.BottomLeft.Y, rect.TopRight.Y);
            

            float centerX = _minX + (_maxX - _minX) / 2;
            float centerY = _minY + (_maxY - _minY) / 2;

            float sizeX = _maxX - centerX;
            float sizeY = _maxY - centerY;

            float minX = centerX * scale.X - sizeX * scale.X;
            float maxX = centerX * scale.X + sizeX * scale.X;
            float minY = centerY * scale.Y - sizeY * scale.Y;
            float maxY = centerY * scale.Y + sizeY * scale.Y;
            
            
            Vector2 position = transform.GetPosition(WorldRelation.Global);


            Vector2 center = new Vector2(centerX, centerY);
            Vector2 bottomLeft  = center * scale + position - new Vector2((center.X - rect.BottomLeft.X)  * scale.X, (center.Y - rect.BottomLeft.Y)  * scale.Y);
            Vector2 bottomRight = center * scale + position - new Vector2((center.X - rect.BottomRight.X) * scale.X, (center.Y - rect.BottomRight.Y) * scale.Y);
            Vector2 topRight    = center * scale + position - new Vector2((center.X - rect.TopRight.X)    * scale.X, (center.Y - rect.TopRight.Y)    * scale.Y);
            Vector2 topLeft     = center * scale + position - new Vector2((center.X - rect.TopLeft.X)     * scale.X, (center.Y - rect.TopLeft.Y)     * scale.Y);

            return new Rectangle(bottomLeft, bottomRight, topRight, topLeft);
        }
    }

    public static class Vector2ExtensionMethods
    {
        public static Vector2 ApplyMatrix2(this Vector2 vec, Matrix2 mat)
        {
            Vector2 newVec = new Vector2(0);

            newVec.X = mat.M11 * vec.X + mat.M12 * vec.Y;
            newVec.Y = mat.M21 * vec.X + mat.M22 * vec.Y;

            return newVec;
        }
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
            return new Transform(matrix * transform.TransformMatrix, transform.GetGlobalRotation());
        }

        public static Transform operator *(Transform transform, Transform other)
        {
            return new Transform(transform.TransformMatrix * other.TransformMatrix, transform.GetGlobalRotation() + other.GetGlobalRotation());
        }
    }


}
