using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConflictCube.GlobalMethods.Audio
{
    public class Audio
    {
        private readonly IWavePlayer outputDevice;
        public readonly MixingSampleProvider mixer;
        

        private List<LoopingAudio> Looping = new List<LoopingAudio>();


        private Audio(int sampleRate = 48000, int channelCount = 2)
        {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();

            new Thread(new ThreadStart(() => { while (true) { Thread.Sleep(50); StartLoopSoundsIfTheyStopped(); } })).Start();
        }

        public void StartLoopSoundsIfTheyStopped()
        {
            if(Looping.Count == 0)
            {
                return;
            }

            foreach(LoopingAudio audio in Looping)
            {
                audio.Playing = false;
            }
            
            foreach(var input in mixer.MixerInputs)
            {
                for(int i = 0; i < Looping.Count; i++)
                {
                    if(Looping[i].Sample == input)
                    {
                        Looping[i].Playing = true;
                        break;
                    }
                } 
            }

            for(int i = 0; i < Looping.Count; i++)
            {
                if(!Looping[i].Playing)
                {
                    Looping[i].Sample = PlaySound(Looping[i].Sound);
                    Looping[i].Playing = true;
                }
            }
        }

        public LoopingAudio LoopSound(CachedSound sound)
        {
            return LoopSound(sound, new CachedSoundSampleProvider(sound));
        }

        private LoopingAudio LoopSound(CachedSound sound, ISampleProvider sample)
        {
            ISampleProvider finalSample = AddMixerInput(sample);
            LoopingAudio looping = new LoopingAudio(sound, sample, true);
            Looping.Add(looping);
            return looping;
        }


        public ISampleProvider PlaySound(string fileName)
        {
            using (var input = new AudioFileReader(fileName))
            {
                return AddMixerInput(input);
            }
        }

        public void StopLoop(LoopingAudio looping)
        {
            StopSound(looping.Sample);
            Looping.Remove(looping);
        }

        public ISampleProvider PlaySound(CachedSound sound)
        {
            return AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        private ISampleProvider AddMixerInput(ISampleProvider input)
        {
            ISampleProvider sample = ConvertToRightChannelCount(input);
            mixer.AddMixerInput(sample);
            return sample;
        }


        public void StopSound(ISampleProvider input)
        {
            RemoveMixerInput(input);
        }
        
        private void RemoveMixerInput(ISampleProvider input)
        {
            mixer.RemoveMixerInput(input);
        }


        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }

        public static readonly Audio Instance = new Audio(48000, 2);

    }
}
