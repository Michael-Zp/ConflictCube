using Engine.Debug;
using System;

namespace Engine.Time
{
    public static class Time
    {
        private static float RealTime = 0;
        private static float _CurrentTime = 0;
        private static float LastSecond = 0;
        private static float FrameCount = 0;
        public static float CurrentTime {
            get {
                return _CurrentTime;
            }

            set {
                DifTime = value - RealTime;
                DifTime *= TimeScale;


                if (DebugEngine.PrintFPS)
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

                _CurrentTime = _CurrentTime + DifTime;
                RealTime = value;
            }
        }

        public static float DifTime { get; set; }
        public static float TimeScale { get; set; } = 1;


        public static bool CooldownIsOver(float LastTime, float Cooldown)
        {
            return CurrentTime - LastTime > Cooldown;
        }
    }
}
