using System;

namespace ConflictCube.ComponentBased.Components
{
    public abstract class Component
    {
        public GameObject Owner;

        public virtual void SetOwner(GameObject owner)
        {
            Owner = owner;
        }

        public virtual Component Clone()
        {
            Component newComponent = (Component)MemberwiseClone();

            newComponent.Owner = Owner;

            return newComponent;
        }
    }
}
