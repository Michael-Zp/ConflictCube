using OpenTK;

namespace Engine.Components
{
    public static class Vector2ExtensionMethods
    {
        public static Vector2 ApplyMatrix2(this Vector2 vec, Matrix2 mat)
        {
            Vector2 newVec = new Vector2()
            {
                X = mat.M11 * vec.X + mat.M12 * vec.Y,
                Y = mat.M21 * vec.X + mat.M22 * vec.Y
            };

            return newVec;
        }
    }
}
