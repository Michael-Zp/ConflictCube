using Engine.Time;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Engine.Components
{
    public class ParticleSystem : Component
    {
#pragma warning disable 0649

        [Import(typeof(ITime))]
        private ITime Time;

#pragma warning restore 0649

        public int ParticleCount;
        public float TimeBetweenParticles;
        private float LastParticleSpawn;
        private float ParticlesToBeSpawned;
        public Material ParticleMaterial;

        public Vector2 OriginalParticleSize;

        public float Lifetime;

        private Func<float, float> _SizeOverTime;
        public Func<float, float> SizeOverTime {
            get {
                if (_SizeOverTime == null)
                {
                    return ((r) => { return 1; });
                }
                return _SizeOverTime;
            }
            set {
                _SizeOverTime = value;
            }
        }

        public Func<float, float> _VelocityOverTime;
        public Func<float, float> VelocityOverTime {
            get {
                if (_VelocityOverTime == null)
                {
                    return ((r) => { return 1; });
                }
                return _VelocityOverTime;
            }
            set {
                _VelocityOverTime = value;
            }
        }

        public Vector2 ParticleFlowDirection;

        /// <summary>
        /// The raidus of the cone, in which direciton the particles will move in degree.
        /// </summary>
        public float FlowConeRadius;

        private List<Particle> Particles = new List<Particle>();
        private Random RandomGen;

        public ParticleSystem(int particleCount, float timeBetweenParticles, Material particleTexture, Vector2 originalParticleSize, float lifetime, Func<float, float> sizeOverTime, Func<float, float> velocityOverTime, Vector2 particleFlowDirection, float flowConeRadius)
        {
            GameEngine.Container.ComposeParts(this);

            ParticleCount = particleCount;
            TimeBetweenParticles = timeBetweenParticles;
            LastParticleSpawn = -TimeBetweenParticles;
            ParticleMaterial = particleTexture;
            OriginalParticleSize = originalParticleSize;
            Lifetime = lifetime;
            SizeOverTime = sizeOverTime;
            VelocityOverTime = velocityOverTime;
            ParticleFlowDirection = particleFlowDirection;
            FlowConeRadius = flowConeRadius;
            RandomGen = new Random((int)DateTime.Now.ToFileTime());
        }


        public override void OnUpdate()
        {
            if (TimeBetweenParticles <= 0)
            {
                ParticlesToBeSpawned = ParticleCount;
            }
            else
            {
                if (Time.CooldownIsOver(LastParticleSpawn, TimeBetweenParticles))
                {
                    ParticlesToBeSpawned += Time.DifTime / TimeBetweenParticles;
                }
            }

            for (int i = Particles.Count; i < (int)Math.Floor(ParticlesToBeSpawned); i++)
            {
                if (Particles.Count >= ParticleCount)
                {
                    break;
                }

                SpawnParticle();
            }

            UpdateParticles();
        }


        private void SpawnParticle()
        {
            float angle;
            if (ParticleFlowDirection == Vector2.UnitY)
            {
                angle = 0;
            }
            else if (ParticleFlowDirection == -Vector2.UnitY)
            {
                angle = 180;
            }
            else
            {
                angle = (float)Math.Acos(Vector2.Dot(Vector2.UnitY, ParticleFlowDirection));
                angle = MathHelper.RadiansToDegrees(angle);
            }

            angle = ParticleFlowDirection.X < 0 ? angle + 180 : angle;

            float minAngle = angle - FlowConeRadius / 2;

            float randomAngle = (float)RandomGen.NextDouble() * FlowConeRadius + minAngle;

            float radianAngle = MathHelper.DegreesToRadians(randomAngle);

            Vector2 randomVelocity = new Vector2((float)Math.Sin(radianAngle), (float)Math.Cos(radianAngle));
            randomVelocity.Normalize();

            Particle newParticle = new Particle("particle " + Particles.Count, new Transform(0, 0, OriginalParticleSize.X, OriginalParticleSize.Y), Owner, this, ParticleMaterial, randomVelocity);

            newParticle.AddComponent(new PhysicsObject(randomVelocity, Vector2.Zero));

            Particles.Add(newParticle);
        }

        private void UpdateParticles()
        {
            foreach (Particle particle in Particles)
            {
                float normalizedAliveTime = (Time.CurrentTime - particle.SpawnTime) / Lifetime;

                float sizeFactor = SizeOverTime(normalizedAliveTime);
                float velocityVector = VelocityOverTime(normalizedAliveTime);

                particle.Transform.SetSize(OriginalParticleSize * sizeFactor, WorldRelation.Local);
                particle.GetComponent<PhysicsObject>().Velocity = particle.OriginalVelocity * velocityVector;
            }
        }

        public void RemoveParticle(Particle particle)
        {
            Particles.Remove(particle);
            ParticlesToBeSpawned--;
        }
    }
}
