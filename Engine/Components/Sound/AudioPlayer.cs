using System.ComponentModel.Composition;
using System.IO;
using Engine.AudioSupport;
using NAudio.Wave;

namespace Engine.Components
{
    public class AudioPlayer : Component
    {
#pragma warning disable 0649

        [Import(typeof(IAudio))]
        private IAudio Audio;

#pragma warning restore 0649

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
            GameEngine.Container.ComposeParts(this);

            Sound = sound;
            Loop = loop;
        }

        public override void OnUpdate()
        {
            if(StartPlayingSoundOnUpdate)
            {
                if (!Loop)
                {
                    SampleProvider = Audio.PlaySound(Sound);
                }
                else
                {
                    LoopingAudio = Audio.LoopSound(Sound);
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
                Audio.StopSound(SampleProvider);
                SampleProvider = null;
            }
            else if(LoopingAudio != null)
            {
                Audio.StopLoop(LoopingAudio);
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
