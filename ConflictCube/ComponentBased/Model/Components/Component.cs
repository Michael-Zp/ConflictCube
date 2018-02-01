namespace ConflictCube.ComponentBased.Components
{
    public abstract class Component
    {
        public GameObject Owner;
        public bool Enabled;

        public Component(bool enabled = true)
        {
            Enabled = enabled;
        }

        public virtual void SetOwner(GameObject owner)
        {
            Owner = owner;
        }

        public virtual void OnRemove()
        {

        }

        public virtual Component Clone()
        {
            Component newComponent = (Component)MemberwiseClone();

            newComponent.Owner = Owner;

            return newComponent;
        }

        public virtual void OnUpdate()
        {

        }
    }
}
