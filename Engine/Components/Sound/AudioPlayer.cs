using System.IO;
using Engine.AudioSuppoert;
using NAudio.Wave;

namespace Engine.Components
{
    public class AudioPlayer : Component
    {
        private CachedSound Sound;
        private ISampleProvider SampleProvider = null;
        private LoopingAudio LoopingAudio = null;
        private bool Loop;

        private bool StartPlayingSoundOnUpdate = false;
        

        public AudioPlayer(byte[] audioResource, bool loop) : this(new CachedSound(audioResource), loop)
        {}

        public AudioPlayer(UnmanagedMemoryStream audioResource, bool loop) : this(new CachedSound(audioResource), loop)
        {}

        private AudioPlayer(CachedSound sound, bool loop)
        {
            Sound = sound;
            Loop = loop;
        }

        public override void OnUpdate()
        {
            if(StartPlayingSoundOnUpdate)
            {
                if (!Loop)
                {
                    SampleProvider = Audio.Instance.PlaySound(Sound);
                }
                else
                {
                    LoopingAudio = Audio.Instance.LoopSound(Sound);
                }
                StartPlayingSoundOnUpdate = false;
            }
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

            StartPlayingSoundOnUpdate = false;
        }

        /// <summary>
        /// Stops the current sound and starts playing the new sound the next time OnUpdate() is called.
        /// </summary>
        public void PlayAudio()
        {
            if(SampleProvider != null || LoopingAudio != null)
            {
                StopAudio();
            }

            StartPlayingSoundOnUpdate = true;
        }
    }
}
