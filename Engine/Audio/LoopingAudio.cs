using NAudio.Wave;

namespace Engine.AudioSuppoert
{
    public class LoopingAudio
    {
        public CachedSound Sound;
        public ISampleProvider Sample;
        public bool Playing;

        public LoopingAudio(CachedSound sound, ISampleProvider sample, bool playing)
        {
            Sound = sound;
            Sample = sample;
            Playing = playing;
        }
    }
}
