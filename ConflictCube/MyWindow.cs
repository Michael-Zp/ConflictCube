﻿using OpenTK;
using System.Diagnostics;

namespace ConflictCube
{
    public class MyWindow : GameWindow
    {
        private float LastTime = 0;

        public MyWindow(int width = 512, int height = 512) : base(width, height)
        {
            Visible = true; //show the window
            globalTime.Start();
        }

        public float GetTime()
        {
            return (float)globalTime.Elapsed.TotalSeconds;
        }

        public float TimeDiff()
        {
            float Now = GetTime();
            float TimeDifference = Now - LastTime;
            LastTime = Now;
            return TimeDifference;
        }

        public bool WaitForNextFrame()
        {
            SwapBuffers(); //double buffering
            ProcessEvents(); //handle all events that are sent to the window (user inputs, operating system stuff); this call could destroy window, so check immediatily after this call if window still exists, otherwise gl calls will fail.
            return Exists;
        }

        private Stopwatch globalTime = new Stopwatch();
    }
}
