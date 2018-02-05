using Engine.Time;
using OpenTK;
using System.ComponentModel.Composition;

namespace Engine.Components
{
    public class Particle : GameObject
    {
#pragma warning disable 0649

        [Import(typeof(ITime))]
        private ITime Time;

#pragma warning restore 0649

        public Vector2 OriginalVelocity;
        public float SpawnTime;

        private ParticleSystem MySystem;

        public Particle(string name, Transform transform, GameObject parent, ParticleSystem mySystem, Material material, Vector2 velocity, bool enabled = true) : base(name, transform, parent, enabled)
        {
            GameEngine.Container.ComposeParts(this);

            SpawnTime = Time.CurrentTime;
            OriginalVelocity = velocity;
            MySystem = mySystem;

            AddComponent(material);
        }

        public override void OnUpdate()
        {
            if(Time.CurrentTime - SpawnTime >= MySystem.Lifetime)
            {
                Destroy(this);
            }
        }

        public override void OnDestroy()
        {
            MySystem.RemoveParticle(this);
        }
    }
}
