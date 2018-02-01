namespace ConflictCube.ComponentBased.Model.Components.Objects.Events
{
    public abstract class Event
    {
        public bool IsStarted { get; set; } = false;

        public abstract void StartEvent();
    }
}
