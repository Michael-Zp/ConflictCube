using Engine.Debug;
using System;
using System.ComponentModel.Composition;

namespace Engine.Time
{
    [Export(typeof(ITime))]
    [Export(typeof(ITimeSetter))]
    public class Time : ITime, ITimeSetter
    {
        private static float RealTime = 0;
        private static float LastSecond = 0;
        private static float FrameCount = 0;

        private static float _CurrentTime = 0;
        public float CurrentTime {
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

        private static float _DifTime = 0;
        public float DifTime {
            get {
                return _DifTime;
            }
            private set {
                _DifTime = value;
            }
        }

        private static float _TimeScale = 1;
        public float TimeScale {
            get {
                return _TimeScale;
            }
            set {
                _TimeScale = value;
            }
        }

        public bool CooldownIsOver(float LastTime, float Cooldown)
        {
            return CurrentTime - LastTime > Cooldown;
        }
    }
}
