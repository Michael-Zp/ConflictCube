using NAudio.Wave;

namespace Engine.AudioSupport
{
    public interface IAudio
    {
        LoopingAudio LoopSound(CachedSound sound);
        ISampleProvider PlaySound(string fileName);
        void StopLoop(LoopingAudio looping);
        ISampleProvider PlaySound(CachedSound sound);
        void StopSound(ISampleProvider input);
    }
}
