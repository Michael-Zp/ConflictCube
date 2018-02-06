namespace Engine.Time
{
    public interface ITime
    {
        float CurrentTime { get; }
        float DifTime { get; }
        float TimeScale { get; set; }
        bool CooldownIsOver(float LastTime, float Cooldown);
    }
}
