using Engine.Components;

namespace ConflictCube.Objects
{
    public class Button : GameObject
    {
        public Event Event;

        private Material DeactivatedMaterial = new Material(System.Drawing.Color.OrangeRed, null, null);
        private Material ActivatedMaterial = new Material(System.Drawing.Color.Green, null, null);

        public Button(string name, Transform transform, Event eventToFireOnActivate, CollisionGroup group, GameObject parent) : base(name, transform, parent)
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
                case "PlayerFire":
                    Event.StartEvent();
                    break;

                case "PlayerIce":
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
