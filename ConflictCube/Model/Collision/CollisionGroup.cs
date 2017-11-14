using ConflictCube.Model.Renderable;
using System.Collections.Generic;
using System;
using Zenseless.Geometry;

namespace ConflictCube.Model.Collision
{
    class CollisionGroup
    {
        public List<ICollidable> CollidersInGroup { get; private set; }
        
        
        public CollisionGroup()
        {
            CollidersInGroup = new List<ICollidable>();
        }

        //Moves all objects in this collision group which are IMoveables and then checks all collisions of all objects
        public void MoveAllObjects()
        {
            List<Tuple<ICollidable, Box2D>> moveBoxes = new List<Tuple<ICollidable, Box2D>>();
            foreach(ICollidable collidable in CollidersInGroup)
            {
                if(collidable is IMoveable)
                {
                    //Console.WriteLine(((IMoveable)collidable).MoveVectorThisIteration);
                    Box2D moveBox = ((IMoveable)collidable).MoveThisIteration(collidable);
                    moveBoxes.Add(Tuple.Create(collidable, moveBox));
                }
            }

            foreach(Tuple<ICollidable, Box2D> collidable in moveBoxes)
            {
                CheckCollisions(collidable);
            }
        }

        private void CheckCollisions(Tuple<ICollidable, Box2D> collidable)
        {
            foreach(ICollidable other in CollidersInGroup)
            {
                if(other == collidable.Item1)
                {
                    continue;
                }

                if(collidable.Item2.Intersects(other.CollisionBox))
                {
                    collidable.Item1.OnCollide(other);
                }
            }
        }
    }
}
