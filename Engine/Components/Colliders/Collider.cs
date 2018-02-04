using OpenTK;
using System.Collections.Generic;

namespace Engine.Components
{
    public abstract class Collider : Component
    {
        public bool IsTrigger;
        public bool IsStatic;
        public CollisionGroup Group;
        public string Type;
        public string Layer;
        public List<string> IgnoreCollisionsWith = new List<string>();

        public float MinX { get; }
        public float MaxX { get; }
        public float MinY { get; }
        public float MaxY { get; }

        public Collider(bool isTrigger, CollisionGroup group, float minX, float maxX, float minY, float maxY) : this(isTrigger, group, "NonCollider", minX, maxX, minY, maxY)
        {}

        public Collider(bool isTrigger, CollisionGroup group, string type, float minX, float maxX, float minY, float maxY, string layer = "Default")
        {
            IsTrigger = isTrigger;
            if(group == null)
            {
                Group = CollisionGroup.DefaultCollisionGroup;
                CollisionGroup.DefaultCollisionGroup.AddCollider(this);
            }
            else
            {
                Group = group;
                Group.AddCollider(this);
            }
            Type = type;
            Layer = layer;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            Group.RemoveCollider(this);
        }

        public virtual void CollidesWith(Collider other, Vector2 movement)
        {
            StandardCollision(other, movement);
            Owner.OnCollision(other);
        }

        public abstract void StandardCollision(Collider other, Vector2 movement);

        public void CheckCollisions(Vector2 movement)
        {
            Group.CheckCollisions(this, movement);
        }

        public abstract bool IsCollidingWith(Collider other);

        public override Component Clone()
        {
            Collider newCollider = (Collider)base.Clone();

            newCollider.IsTrigger = IsTrigger;
            newCollider.Group = Group;
            newCollider.Type = Type;

            return newCollider;
        }
    }
}
