using OpenTK;

namespace Engine.Components
{
    public class Particle : GameObject
    {
        public Vector2 OriginalVelocity;
        public float SpawnTime;

        private ParticleSystem MySystem;

        public Particle(string name, Transform transform, GameObject parent, ParticleSystem mySystem, Material material, Vector2 velocity, bool enabled = true) : base(name, transform, parent, enabled)
        {
            SpawnTime = Time.Time.CurrentTime;
            OriginalVelocity = velocity;
            MySystem = mySystem;

            AddComponent(material);
        }

        public override void OnUpdate()
        {
            if(Time.Time.CurrentTime - SpawnTime >= MySystem.Lifetime)
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
