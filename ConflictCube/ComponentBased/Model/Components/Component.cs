﻿namespace ConflictCube.ComponentBased.Components
{
    public abstract class Component
    {
        public GameObject Owner;
        public bool Enabled;

        public virtual void SetOwner(GameObject owner, bool enabled = true)
        {
            Owner = owner;
            Enabled = enabled;
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
    }
}
