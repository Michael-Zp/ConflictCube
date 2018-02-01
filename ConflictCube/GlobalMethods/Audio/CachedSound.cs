using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConflictCube.GlobalMethods.Audio
{
    public class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        
        public CachedSound(string audioFileName)
        {
            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                Initalized(audioFileReader, audioFileReader.Length);
            }
        }
        
        public CachedSound(byte[] audioFile)
        {
            using (var stream = new Mp3FileReader(new MemoryStream(audioFile)))
            {
                Initalized(stream.ToSampleProvider(), stream.Length);
            }
        }

        public CachedSound(UnmanagedMemoryStream birdWhistle)
        {
            using (var stream = new WaveFileReader(birdWhistle))
            {
                Initalized(stream.ToSampleProvider(), stream.Length);
            }
        }

        private void Initalized(ISampleProvider provider, long byteLength)
        {
            WaveFormat = provider.WaveFormat;
            var wholeFile = new List<float>((int)(byteLength / 4));
            var readBuffer = new float[provider.WaveFormat.SampleRate * provider.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = provider.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            AudioData = wholeFile.ToArray();
        }
        
    }
}
