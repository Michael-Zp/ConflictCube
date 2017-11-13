using System.Collections.Generic;

namespace ConflictCube.Model.Collision
{
    class CollisionGroup
    {
        public List<ICollidable> CollidersInGroup { get; private set; }

        public CollisionGroup()
        {
            CollidersInGroup = new List<ICollidable>();
        }

        public void CheckCollisions(ICollidable collidable)
        {
            foreach(ICollidable other in CollidersInGroup)
            {
                if(collidable.CollisionBox.Intersects(other.CollisionBox))
                {
                    collidable.OnCollide(other)
                }
            }
        }
    }
}
