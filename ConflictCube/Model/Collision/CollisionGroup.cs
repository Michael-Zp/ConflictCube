using ConflictCube.Model.Renderable;
using System.Collections.Generic;
using System;
using OpenTK;

namespace ConflictCube.Model.Collision
{
    public class CollisionGroup
    {
        public List<ICollidable> CollidersInGroup { get; private set; }
        
        
        public CollisionGroup()
        {
            CollidersInGroup = new List<ICollidable>();
        }

        public void AddCollider(ICollidable collidable)
        {
            CollidersInGroup.Add(collidable);
            collidable.CollisionGroup = this;
        }

        public void AddRangeColliders(IEnumerable<ICollidable> collidables)
        {
            foreach(ICollidable collidable in collidables)
            {
                AddCollider(collidable);
            }
        }

        //Moves all objects in this collision group which are IMoveables and then checks all collisions of all objects
        public void MoveAllObjects()
        {
            foreach(ICollidable collidable in CollidersInGroup)
            {
                if(collidable is IMoveable)
                {
                    ((IMoveable)collidable).MoveThisIteration();
                }
            }

            CheckCollisions();

            foreach(ICollidable collidable in CollidersInGroup)
            {
                if(collidable is IMoveable)
                {
                    ((IMoveable)collidable).ClearMoveVector();
                }
            }
        }

        private void CheckCollisions()
        {
            foreach(ICollidable obj in CollidersInGroup)
            {
                foreach(ICollidable other in CollidersInGroup)
                {
                    if (other == obj)
                    {
                        continue;
                    }

                    if (obj.CollisionBox.Intersects(other.CollisionBox))
                    {
                        if(!obj.IsTrigger() && !other.IsTrigger() && obj is IMoveable)
                        {
                            UnstuckTwoColliders(obj, other, (IMoveable)obj);
                        }
                        obj.OnCollide(other);
                    }
                }
            }
        }

        private void UnstuckTwoColliders(ICollidable obj, ICollidable other, IMoveable objMoveable)
        {
            Vector2 onlyXMovement = new Vector2(objMoveable.MoveVectorThisIteration.X, 0f);
            Vector2 onlyYMovement = new Vector2(0f, objMoveable.MoveVectorThisIteration.Y);
            
            objMoveable.MoveInstantly(-objMoveable.MoveVectorThisIteration);

            objMoveable.MoveInstantly(onlyXMovement);

            if (other.CollisionBox.Intersects(obj.CollisionBox))
            {
                float yDistance = Math.Abs(Math.Abs(other.CollisionBox.CenterY - obj.CollisionBox.CenterY) - other.CollisionBox.SizeY / 2 - obj.CollisionBox.SizeY / 2);

                if (yDistance > 0.001f)
                {
                    float xDif = 0;
                    if (onlyXMovement.X > 0)
                    {
                        xDif = ((other.CollisionBox.MinX - obj.CollisionBox.SizeX) + 0.0000001f) - obj.CollisionBox.MinX;
                    }
                    else if (onlyXMovement.X < 0)
                    {
                        xDif = (other.CollisionBox.MaxX - 0.0000001f) - obj.CollisionBox.MinX;
                    }
                    objMoveable.MoveInstantly(new Vector2(xDif, 0f));
                }
            }


            objMoveable.MoveInstantly(onlyYMovement);

            if (other.CollisionBox.Intersects(obj.CollisionBox))
            {
                float xDistance = Math.Abs(Math.Abs(other.CollisionBox.CenterX - obj.CollisionBox.CenterX) - other.CollisionBox.SizeX / 2 - obj.CollisionBox.SizeX / 2);

                if (xDistance > 0.001f)
                {
                    float yDif = 0;
                    if (onlyYMovement.Y > 0)
                    {
                        yDif = ((other.CollisionBox.MinY - obj.CollisionBox.SizeY) + 0.0000001f) - obj.CollisionBox.MinY;
                    }
                    else if (onlyYMovement.Y < 0)
                    {
                        yDif = (other.CollisionBox.MaxY - 0.0000001f) - obj.CollisionBox.MinY;
                    }
                    objMoveable.MoveInstantly(new Vector2(0f, yDif));
                }
            }
        }
    }
}
