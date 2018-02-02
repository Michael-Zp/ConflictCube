using System.Drawing;

namespace ConflictCube.ComponentBased.Model.Components.Animators
{
    public class MaterialAnimatorKeyframe
    {
        public float Time;

        public float Alpha;
        public float Red;
        public float Green;
        public float Blue;

        public MaterialAnimatorKeyframe(float time, float alpha, float red, float green, float blue)
        {
            Time = time;
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public MaterialAnimatorKeyframe(float time, Color color) : this(time, color.A, color.R, color.G, color.B)
        {}
    }
}
