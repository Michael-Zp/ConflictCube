using System.Collections.Generic;
using ConflictCube.ComponentBased.Model.Components.Colliders;
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
            foreach (Collider collider in colliders)
            {
                AddCollider(collider);
            }
        }

        public void CheckCollisions(Collider collider, Vector2 movement)
        {
            if (!collider.Enabled)
            {
                return;
            }

            //No foreach loop, because events in the CollidesWIth could change the CollidersInGroup list -> Throws exception
            for (int i = 0; i < CollidersInGroup.Count; i++)
            {
                if (CollidersInGroup[i] == collider || !collider.Owner.EnabledInHierachy || !CollidersInGroup[i].Owner.EnabledInHierachy)
                {
                    continue;
                }

                if (!CollidersInGroup[i].Enabled)
                {
                    continue;
                }

                if (!collider.Layer.AreLayersColliding(CollidersInGroup[i].Layer))
                {
                    continue;
                }

                if (collider.IgnoreCollisionsWith.Contains(CollidersInGroup[i].Type))
                {
                    continue;
                }

                if (collider.IsCollidingWith(CollidersInGroup[i]))
                {
                    collider.CollidesWith(CollidersInGroup[i], movement);

                    if (CollidersInGroup[i].IsCollidingWith(collider))
                    {
                        CollidersInGroup[i].CollidesWith(collider, new Vector2(0, 0));
                    }
                }
            }
        }

        public void RemoveCollider(Collider colliderToRemove)
        {
            CollidersInGroup.Remove(colliderToRemove);
        }
    }
}
