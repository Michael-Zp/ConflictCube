using System;
using System.Threading;
using ConflictCube.GlobalMethods.Audio;

namespace ZenselessTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var sound = new CachedSound(Resources.BirdWhistle);

            var song = Audio.Instance.LoopSound(sound);
            

            for(int i = 0; i < 20; i++)
            {
                Thread.Sleep(1000);

                Console.WriteLine("---");
                foreach(var input in Audio.Instance.mixer.MixerInputs)
                {
                    Console.WriteLine(input.ToString());
                }
                Console.WriteLine("---");
            }

            Audio.Instance.StopSound(song);


            for (int i = 0; i < 4; i++)
            {
                Thread.Sleep(1000);

                Console.WriteLine("---");
                foreach (var input in Audio.Instance.mixer.MixerInputs)
                {
                    Console.WriteLine(input.ToString());
                }
                Console.WriteLine("---");
            }
        }
    }
}

