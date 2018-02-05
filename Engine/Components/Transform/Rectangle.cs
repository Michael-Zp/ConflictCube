using OpenTK;

namespace Engine.Components
{
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
}
