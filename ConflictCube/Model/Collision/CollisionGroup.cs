using ConflictCube.Model.Renderable;
using System.Collections.Generic;

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
                        obj.OnCollide(other);
                    }
                }
            }
        }
    }
}
