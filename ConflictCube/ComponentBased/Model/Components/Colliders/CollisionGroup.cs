using System.Collections.Generic;
using OpenTK;

namespace ConflictCube.ComponentBased.Components
{
    public class CollisionGroup
    {
        private List<Collider> _CollidersInGroup = new List<Collider>();
        public List<Collider> CollidersInGroup {
            get {
                return _CollidersInGroup;
            }
            private set {
                _CollidersInGroup = value;
            }
        }

        public static CollisionGroup DefaultCollisionGroup { get; private set; } = new CollisionGroup();
        
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
            foreach (Collider other in CollidersInGroup)
            {
                if (other == collider || !collider.Owner.EnabledInHierachy || !other.Owner.EnabledInHierachy)
                {
                    continue;
                }

                if (collider.IsCollidingWith(other))
                {
                    collider.CollidesWith(other, movement);
                }
            }
        }

        public void RemoveCollider(Collider colliderToRemove)
        {
            CollidersInGroup.Remove(colliderToRemove);
        }
    }
}
