using System.IO;
using ConflictCube.ComponentBased.Components;
using ConflictCube.GlobalMethods.Audio;
using NAudio.Wave;

namespace ConflictCube.ComponentBased.Model.Components.Sound
{
    public class AudioPlayer : Component
    {
        private CachedSound Sound;
        private ISampleProvider SampleProvider = null;
        private LoopingAudio LoopingAudio = null;
        private bool Loop;

        public AudioPlayer(byte[] audioResource, bool loop) : this(new CachedSound(audioResource), loop)
        {}

        public AudioPlayer(UnmanagedMemoryStream audioResource, bool loop) : this(new CachedSound(audioResource), loop)
        {}

        private AudioPlayer(CachedSound sound, bool loop)
        {
            Sound = sound;
            Loop = loop;
        }

        public override void OnRemove()
        {
            StopAudio();
        }

        public void StopAudio()
        {
            if(SampleProvider != null)
            {
                Audio.Instance.StopSound(SampleProvider);
                SampleProvider = null;
            }
            else if(LoopingAudio != null)
            {
                Audio.Instance.StopLoop(LoopingAudio);
                LoopingAudio = null;
            }
        }

        public void PlayAudio()
        {
            if(SampleProvider != null || LoopingAudio != null)
            {
                StopAudio();
            }

            if(!Loop)
            {
                SampleProvider = Audio.Instance.PlaySound(Sound);
            }
            else
            {
                LoopingAudio = Audio.Instance.LoopSound(Sound);
            }
        }
    }
}
