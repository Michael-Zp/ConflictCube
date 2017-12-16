using OpenTK.Input;
using System;
using System.Threading;

namespace ZenselessTests
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Up -> " + GamePad.GetState(0).Buttons.A);
                Console.WriteLine("Right -> " + GamePad.GetState(0).Buttons.B);
                Console.WriteLine("Down -> " + GamePad.GetState(0).Buttons.X);
                Console.WriteLine("Left -> " + GamePad.GetState(0).Buttons.Y);
                Console.WriteLine("Place -> " + GamePad.GetState(0).Buttons.Back);
                Console.WriteLine("SwitchMode -> " + GamePad.GetState(0).Buttons.Start);
                Console.WriteLine("Move X -> " + GamePad.GetState(0).ThumbSticks.Left.X);
                Console.WriteLine("Move Y -> " + GamePad.GetState(0).ThumbSticks.Left.Y);

                Thread.Sleep(100);
                Console.Clear();
            }
        }
    }
}
