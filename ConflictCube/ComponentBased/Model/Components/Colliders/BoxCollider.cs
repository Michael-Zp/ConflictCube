using OpenTK;
using System;

namespace ConflictCube.ComponentBased.Components
{
    public class BoxCollider : Collider
    {
        public Transform Box;

        public BoxCollider(Transform transform, bool isTrigger, CollisionGroup group) : base(isTrigger, group)
        {
            Box = transform;
        }

        public BoxCollider(Transform transform, bool isTrigger, CollisionGroup group, CollisionType type) : base(isTrigger, group, type)
        {
            Box = transform;
        }


        public override bool IsCollidingWith(Collider other)
        {
            if(other is BoxCollider)
            {
                return Box.Intersects(((BoxCollider)other).Box);
            }

            Console.WriteLine("Collider type of other in BoxCollider not known. Other owner is " + other.Owner.Name);
            return false;
        }

        public override void StandardCollision(Collider other, Transform transform, Vector2 movement)
        {
            if(!IsTrigger && !other.IsTrigger)
            {
                if (other is BoxCollider)
                {
                    BoxCollider otherBox = (BoxCollider)other;

                    Vector2 onlyXMovement = new Vector2(movement.X, 0f);
                    Vector2 onlyYMovement = new Vector2(0f, movement.Y);

                    transform.Position -= movement;

                    transform.Position += onlyXMovement;

                    if (otherBox.Box.Intersects(Box))
                    {
                        float yDistance = Math.Abs(Math.Abs(otherBox.Box.Position.Y - Box.Position.Y) - otherBox.Box.Size.Y / 2 - Box.Size.Y / 2);

                        if (yDistance > 0.001f)
                        {
                            float xDif = 0;
                            if (onlyXMovement.X > 0)
                            {
                                xDif = ((otherBox.Box.MinX - Box.Size.X) + 0.0000001f) - Box.MinX;
                            }
                            else if (onlyXMovement.X < 0)
                            {
                                xDif = (otherBox.Box.MaxX - 0.0000001f) - Box.MinX;
                            }
                            transform.Position += new Vector2(xDif, 0f);
                        }
                    }


                    transform.Position += onlyYMovement;

                    if (otherBox.Box.Intersects(Box))
                    {
                        float xDistance = Math.Abs(Math.Abs(otherBox.Box.Position.X - Box.Position.X) - otherBox.Box.Size.X / 2 - Box.Size.X / 2);

                        if (xDistance > 0.001f)
                        {
                            float yDif = 0;
                            if (onlyYMovement.Y > 0)
                            {
                                yDif = ((otherBox.Box.MinY - Box.Size.Y) + 0.0000001f) - Box.MinY;
                            }
                            else if (onlyYMovement.Y < 0)
                            {
                                yDif = (otherBox.Box.MaxY - 0.0000001f) - Box.MinY;
                            }
                            transform.Position += new Vector2(0f, yDif);
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
