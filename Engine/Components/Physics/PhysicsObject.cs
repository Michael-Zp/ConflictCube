using OpenTK;

namespace Engine.Components
{
    public class PhysicsObject : Component
    {
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public PhysicsObject(Vector2 velocity, Vector2 acceleration)
        {
            Velocity = velocity;
            Acceleration = acceleration;
        }

        public override void OnUpdate()
        {
            Velocity += Acceleration * Time.Time.DifTime;
            Owner.Transform.SetPosition(Owner.Transform.GetPosition(WorldRelation.Local) + Velocity * Time.Time.DifTime, WorldRelation.Local);
        }
    }
}
