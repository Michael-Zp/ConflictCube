using NAudio.Wave;
using System.IO;
using System.Threading;

namespace ZenselessTests
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var audio = new Mp3FileReader(new MemoryStream(Resources._05___Manowar)))
            using (var oDevice = new WaveOutEvent())
            {
                oDevice.Init(audio);
                oDevice.Play();
                while (oDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100000);
                }
            }
        }
    }
}

