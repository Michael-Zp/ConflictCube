using System.Collections.Generic;
using OpenTK;

namespace ConflictCube.ComponentBased.Components
{
    public class CollisionGroup
    {
        public List<Collider> CollidersInGroup { get; private set; }

        public static CollisionGroup DefaultCollisionGroup { get; private set; } = new CollisionGroup();

        public CollisionGroup()
        {
            CollidersInGroup = new List<Collider>();
        }

        public void AddCollider(Collider collider)
        {
            CollidersInGroup.Add(collider);
            collider.Group = this;
        }

        public void AddRangeColliders(IEnumerable<Collider> colliders)
        {
            foreach(Collider collider in colliders)
            {
                AddCollider(collider);
            }
        }

        public void CheckCollisions(Collider collider, Vector2 movement)
        {
            foreach(Collider other in CollidersInGroup)
            {
                if (other == collider)
                {
                    continue;
                }

                if (collider.IsCollidingWith(other))
                {
                    collider.CollidesWith(other, collider.Owner.Transform, movement);
                }
            }
        }
    }
}
