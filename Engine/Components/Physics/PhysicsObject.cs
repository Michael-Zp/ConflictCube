using Engine.Time;
using OpenTK;
using System.ComponentModel.Composition;

namespace Engine.Components
{
    public class PhysicsObject : Component
    {
#pragma warning disable 0649

        [Import(typeof(ITime))]
        private ITime Time;

#pragma warning restore 0649

        public Vector2 Velocity;
        public Vector2 Acceleration;

        public PhysicsObject(Vector2 velocity, Vector2 acceleration)
        {
            GameEngine.Container.ComposeParts(this);

            Velocity = velocity;
            Acceleration = acceleration;
        }

        public override void OnUpdate()
        {
            Velocity += Acceleration * Time.DifTime;
            Owner.Transform.SetPosition(Owner.Transform.GetPosition(WorldRelation.Local) + Velocity * Time.DifTime, WorldRelation.Local);
        }
    }
}
