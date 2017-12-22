using OpenTK;
using System;

namespace ConflictCube.ComponentBased.Components
{
    public class BoxCollider : Collider
    {
        public Transform Box;

        public BoxCollider(Transform transform, bool isTrigger, CollisionGroup group) : this(transform, isTrigger, group, CollisionType.NonCollider)
        {}

        public BoxCollider(Transform transform, bool isTrigger, CollisionGroup group, CollisionType type) : base(isTrigger, group, type)
        {
            Box = transform;
        }


        public override bool IsCollidingWith(Collider other)
        {
            if(other is BoxCollider)
            {
                //Because a component holds another component, transform the Box into the space of the owner and then into the global space. Otherwise the transformation into owner space would be missing.
                Transform thisGlobalTransform = GetGlobalCollisionBox();
                BoxCollider otherBoxCollider = ((BoxCollider)other);
                Transform otherGlobalTransform = otherBoxCollider.GetGlobalCollisionBox();
                return thisGlobalTransform.Intersects(otherGlobalTransform);
            }

            Console.WriteLine("Collider type of other in BoxCollider not known. Other owner is " + other.Owner.Name);
            return false;
        }

        public override void SetOwner(GameObject owner)
        {
            base.SetOwner(owner);
            Box.SetOwner(owner);
        }

        private Transform GetGlobalCollisionBox()
        {
            return Box.TransformToSpace(Owner.Transform).TransformToGlobal();
        }

        public override void StandardCollision(Collider other, Vector2 movement)
        {
            if(!IsTrigger && !other.IsTrigger)
            {
                if (other is BoxCollider)
                {
                    Transform thisGlobalBox = Box.Owner.Transform.TransformToGlobal();
                    Transform otherGlobalBox = ((BoxCollider)other).Box.Owner.Transform.TransformToGlobal();

                    if(DebugGame.DrawBoxColliderCollisions)
                    {
                        GameView.DrawDebug(thisGlobalBox, System.Drawing.Color.FromArgb(128, 255, 0, 0));
                        GameView.DrawDebug(otherGlobalBox, System.Drawing.Color.FromArgb(128, 0, 255, 0));
                    }
                    
                    Owner.Transform.AddToPosition(-movement, WorldRelation.Global);


                    if (movement.X > 0)
                    {
                        Owner.Transform.SetPosition(new Vector2(otherGlobalBox.GetMinX(WorldRelation.Global) - thisGlobalBox.GetSize(WorldRelation.Global).X, Owner.Transform.GetPosition(WorldRelation.Global).Y), WorldRelation.Global);
                    }
                    else if (movement.X < 0)
                    {
                        Owner.Transform.SetPosition(new Vector2(otherGlobalBox.GetMaxX(WorldRelation.Global) + thisGlobalBox.GetSize(WorldRelation.Global).X, Owner.Transform.GetPosition(WorldRelation.Global).Y), WorldRelation.Global);
                    }



                    if (movement.Y > 0)
                    {
                        Owner.Transform.SetPosition(new Vector2(Owner.Transform.GetPosition(WorldRelation.Global).X, otherGlobalBox.GetMinY(WorldRelation.Global) - thisGlobalBox.GetSize(WorldRelation.Global).Y), WorldRelation.Global);
                    }
                    else if (movement.Y < 0)
                    {
                        Owner.Transform.SetPosition(new Vector2(Owner.Transform.GetPosition(WorldRelation.Global).X, otherGlobalBox.GetMaxY(WorldRelation.Global) + thisGlobalBox.GetSize(WorldRelation.Global).Y), WorldRelation.Global);
                    }
                }
            }

        }
        
        public override Component Clone()
        {
            BoxCollider newBoxCollider = (BoxCollider)base.Clone();

            newBoxCollider.Box = (Transform)Box.Clone();

            return newBoxCollider;
        }
    }
}
