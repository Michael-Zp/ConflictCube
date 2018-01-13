using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.Objects.Events;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class Button : GameObject
    {
        public Event Event;

        private Material DeactivatedMaterial = new Material(System.Drawing.Color.OrangeRed, null, null);
        private Material ActivatedMaterial = new Material(System.Drawing.Color.Green, null, null);

        public Button(string name, Transform transform, Event eventToFireOnActivate, CollisionGroup group) : base(name, transform)
        {
            Event = eventToFireOnActivate;
            AddComponent(DeactivatedMaterial);
            AddComponent(new BoxCollider(new Transform(), true, group));
        }

        public override void OnCollision(Collider other)
        {
            base.OnCollision(other);

            bool eventWasStarted = Event.IsStarted;
            
            switch(other.Type)
            {
                case CollisionType.PlayerFire:
                    Event.StartEvent();
                    break;

                case CollisionType.PlayerIce:
                    Event.StartEvent();
                    break;
            }

            if(!eventWasStarted && Event.IsStarted)
            {
                RemoveComponent<Material>();
                AddComponent(ActivatedMaterial);
            }
        }
    }
}
