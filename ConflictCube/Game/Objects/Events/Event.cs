namespace ConflictCube.Objects
{
    public abstract class Event
    {
        public bool IsStarted { get; set; } = false;

        public abstract void StartEvent();
    }
}
