using ConflictCube.ComponentBased.Components;
using OpenTK;
using System;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Model.Components.Animators
{
    public class MaterialAnimator : Component
    {
        public const float UseOriginalValue = -100000;

        private bool AnimationIsStarted = false;
        private float StartTime = 0;
        private MaterialAnimatorKeyframe LastKeyframe;
        private int CurrentKeyframeIndex = 0;
        private Material MateraialToAnimate;

        private List<MaterialAnimatorKeyframe> Keyframes = new List<MaterialAnimatorKeyframe>();

        public MaterialAnimator(Material materialToAnimate)
        {
            MateraialToAnimate = materialToAnimate;
        }

        public override void OnUpdate()
        {
            if (AnimationIsStarted && CurrentKeyframeIndex < Keyframes.Count)
            {
                MaterialAnimatorKeyframe currentKeyframe = Keyframes[CurrentKeyframeIndex];

                float timeInAnimation = Time.Time.CurrentTime - StartTime - LastKeyframe.Time;

                float linearRatio;
                if(currentKeyframe.Time - LastKeyframe.Time == 0)
                {
                    linearRatio = 1;
                }
                else
                {
                    linearRatio = timeInAnimation / (currentKeyframe.Time - LastKeyframe.Time);
                }

                linearRatio = MathHelper.Clamp(linearRatio, 0, 1);

                float newA = LastKeyframe.Alpha + (currentKeyframe.Alpha - LastKeyframe.Alpha) * linearRatio;
                float newR = LastKeyframe.Red   + (currentKeyframe.Red   - LastKeyframe.Red)   * linearRatio;
                float newG = LastKeyframe.Green + (currentKeyframe.Green - LastKeyframe.Green) * linearRatio;
                float newB = LastKeyframe.Blue  + (currentKeyframe.Blue  - LastKeyframe.Blue)  * linearRatio;

                MateraialToAnimate.Color = System.Drawing.Color.FromArgb((int)newA, (int)newR, (int)newG, (int)newB);

                if (linearRatio >= 1f - 0.0001f)
                {
                    LastKeyframe = currentKeyframe;
                    CurrentKeyframeIndex++;
                }
            }
        }

        public void StopAnimation()
        {
            AnimationIsStarted = false;
        }

        public void StartAnimation()
        {
            if(AnimationIsStarted)
            {
                return;
            }

            StartTime = Time.Time.CurrentTime;
            AnimationIsStarted = true;
            LastKeyframe = new MaterialAnimatorKeyframe(0f, MateraialToAnimate.Color);
            CurrentKeyframeIndex = 0;
        }

        public void AddKeyframe(float time, float alpha = UseOriginalValue, float red = UseOriginalValue, float green = UseOriginalValue, float blue = UseOriginalValue)
        {
            if(AnimationIsStarted)
            {
                Console.WriteLine("Animation is already started. No keyframe can be added.");
                return;
            }

            foreach (MaterialAnimatorKeyframe keyframe in Keyframes)
            {
                if (keyframe.Time == time)
                {
                    Console.WriteLine("Keyframe at same point was added already. Keyframe will not be added.");
                    return;
                }
            }

            if (alpha == UseOriginalValue)
            {
                alpha = MateraialToAnimate.Color.A;
            }

            if (red == UseOriginalValue)
            {
                red = MateraialToAnimate.Color.R;
            }

            if (green == UseOriginalValue)
            {
                green = MateraialToAnimate.Color.G;
            }

            if (blue == UseOriginalValue)
            {
                blue = MateraialToAnimate.Color.B;
            }

            alpha = MathHelper.Clamp(alpha, 0, 255);
            red = MathHelper.Clamp(red, 0, 255);
            green = MathHelper.Clamp(green, 0, 255);
            blue = MathHelper.Clamp(blue, 0, 255);

            Keyframes.Add(new MaterialAnimatorKeyframe(time, alpha, red, green, blue));

            Keyframes.Sort(new System.Comparison<MaterialAnimatorKeyframe>((one, two) => { return one.Time < two.Time ? -1 : 1; }));
        }
    }
}
