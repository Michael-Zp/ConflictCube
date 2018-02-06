using OpenTK;
using System;

namespace Engine.Components
{
    public static class RectangleExtensionMethods
    {
        public static Rectangle Rotate(this Rectangle rect, float angle, RotationMode mode)
        {
            if (angle == 0)
            {
                return rect;
            }

            if (mode == RotationMode.Degree)
            {
                angle = Zenseless.Geometry.MathHelper.DegreesToRadians(angle);
            }

            Matrix2 rotationMatrix = Matrix2.CreateRotation(angle);


            Vector2 offsetToCenterPoints = new Vector2(-rect.BottomLeft.X - (rect.BottomRight.X - rect.BottomLeft.X) / 2, -rect.BottomLeft.Y - (rect.TopLeft.Y - rect.BottomLeft.Y) / 2);

            rect.BottomLeft += offsetToCenterPoints;
            rect.BottomRight += offsetToCenterPoints;
            rect.TopRight += offsetToCenterPoints;
            rect.TopLeft += offsetToCenterPoints;

            rect.BottomLeft = rect.BottomLeft.ApplyMatrix2(rotationMatrix);
            rect.BottomRight = rect.BottomRight.ApplyMatrix2(rotationMatrix);
            rect.TopRight = rect.TopRight.ApplyMatrix2(rotationMatrix);
            rect.TopLeft = rect.TopLeft.ApplyMatrix2(rotationMatrix);

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
            Vector2 bottomLeft = center * scale + position - new Vector2((center.X - rect.BottomLeft.X) * scale.X, (center.Y - rect.BottomLeft.Y) * scale.Y);
            Vector2 bottomRight = center * scale + position - new Vector2((center.X - rect.BottomRight.X) * scale.X, (center.Y - rect.BottomRight.Y) * scale.Y);
            Vector2 topRight = center * scale + position - new Vector2((center.X - rect.TopRight.X) * scale.X, (center.Y - rect.TopRight.Y) * scale.Y);
            Vector2 topLeft = center * scale + position - new Vector2((center.X - rect.TopLeft.X) * scale.X, (center.Y - rect.TopLeft.Y) * scale.Y);

            return new Rectangle(bottomLeft, bottomRight, topRight, topLeft);
        }
    }
}
