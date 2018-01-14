﻿using ConflictCube.ComponentBased.Model.Components.Colliders;
using OpenTK;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Components
{
    public enum CollisionType
    {
        LeftBoundary,
        RightBoundary,
        TopBoundary,
        BottomBoundary,
        Finish,
        Wall,
        Hole,
        NonCollider,
        PlayerFire,
        PlayerIce,
        OrangeBlock,
        BlueBlock,
        OrangeFloor,
        BlueFloor
    }

    public abstract class Collider : Component
    {
        public bool IsTrigger;
        public CollisionGroup Group;
        public CollisionType Type;
        public CollisionLayer Layer;
        public List<CollisionType> IgnoreCollisionsWith = new List<CollisionType>();

        public Collider(bool isTrigger, CollisionGroup group) : this(isTrigger, group, CollisionType.NonCollider)
        {}

        public Collider(bool isTrigger, CollisionGroup group, CollisionType type, CollisionLayer layer = CollisionLayer.Default)
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
