using ConflictCube.ComponentBased;
using System;

namespace ConflictCube.Time
{
    public class Time
    {
        private static float _CurrentTime = 0;
        private static float LastSecond = 0;
        private static float FrameCount = 0;
        public static float CurrentTime {
            get {
                return _CurrentTime;
            }

            set {
                DifTime = value - _CurrentTime;

                if (DebugGame.PrintFPS)
                {
                    float thisSecond = (float)Math.Floor(_CurrentTime);
                    FrameCount++;

                    if (thisSecond > LastSecond)
                    {
                        Console.WriteLine("FPS: " + FrameCount);

                        LastSecond = thisSecond;
                        FrameCount = 0;
                    }
                }

                _CurrentTime = value;
            }
        }

        public static float DifTime { get; set; }


        public static bool CooldownIsOver(float LastTime, float Cooldown)
        {
            return CurrentTime - LastTime > Cooldown;
        }
    }
}
