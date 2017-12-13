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

        public override void StandardCollision(Collider other, Transform transform, Vector2 movement)
        {
            if(!IsTrigger && !other.IsTrigger)
            {
                if (other is BoxCollider)
                {
                    const float smallEpsilon = 0.0000001f;
                    const float bigEpsilon = 0.02f;

                    BoxCollider otherBox = (BoxCollider)other;

                    Transform thisGlobalBox = GetGlobalCollisionBox();
                    Transform otherGlobalBox = otherBox.GetGlobalCollisionBox();

                    Vector2 onlyXMovement = new Vector2(movement.X, 0f);
                    Vector2 onlyYMovement = new Vector2(0f, movement.Y);

                    Owner.Transform.Position -= movement;
                    
                    


                    Owner.Transform.Position += onlyXMovement;

                    if (otherBox.Box.Intersects(Box))
                    {
                        float yDistance = Math.Abs(Math.Abs(otherGlobalBox.Position.Y - thisGlobalBox.Position.Y) - otherGlobalBox.Size.Y - thisGlobalBox.Size.Y);
                        
                        if (yDistance > bigEpsilon)
                        {
                            float xDif = 0;
                            if (onlyXMovement.X > 0)
                            {
                                xDif = otherGlobalBox.MinX - thisGlobalBox.MaxX + smallEpsilon;
                                
                                
                                //Worked in old code like that, but is a bit complicated and did not work with class Transform...
                                //xDif = ((otherGlobalBox.MinX - thisGlobalBox.Size.X) + smallEpsilon) - thisGlobalBox.MinX; 
                            }
                            else if (onlyXMovement.X < 0)
                            {
                                xDif = (otherGlobalBox.MaxX - smallEpsilon) - thisGlobalBox.MinX;
                            }

                            if(Math.Abs(xDif) <= Math.Abs(onlyXMovement.X) + bigEpsilon)
                            {
                                Owner.Transform.Position += new Vector2(xDif, 0f);
                            }
                        }
                    }


                    Owner.Transform.Position += onlyYMovement;

                    if (otherBox.Box.Intersects(Box))
                    {
                        float xDistance = Math.Abs(Math.Abs(otherGlobalBox.Position.X - thisGlobalBox.Position.X) - otherGlobalBox.Size.X - thisGlobalBox.Size.X);

                        if (xDistance > bigEpsilon)
                        {
                            float yDif = 0;
                            if (onlyYMovement.Y > 0)
                            {
                                yDif = otherGlobalBox.MinY - thisGlobalBox.MaxY + smallEpsilon;


                                //Worked in old code like that, but is a bit complicated and did not work with class Transform...
                                //yDif = ((otherGlobalBox.MinY - thisGlobalBox.Size.Y) + smallEpsilon) - thisGlobalBox.MinY;
                            }
                            else if (onlyYMovement.Y < 0)
                            {
                                yDif = (otherGlobalBox.MaxY - smallEpsilon) - thisGlobalBox.MinY;
                            }

                            if(Math.Abs(yDif) <= Math.Abs(onlyYMovement.Y) + bigEpsilon)
                            {
                                Owner.Transform.Position += new Vector2(0f, yDif);
                            }
                        }
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
