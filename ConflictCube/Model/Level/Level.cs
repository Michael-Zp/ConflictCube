using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using System.Collections.Generic;
using System;

namespace ConflictCube
{
    public class Level
    {
        public Floor FloorRight { get; set; }
        public Floor FloorMiddle { get; set; }
        public Floor FloorLeft { get; set; }
        public float FloorOffsetPerSecond { get; set; }


        public List<ICollidable> GetColliders()
        {
            List<ICollidable> colliders = new List<ICollidable>();

            foreach(RenderableObject obj in FloorRight.ObjectsToRender)
            {
                if (obj is ICollidable)
                {
                    colliders.Add((ICollidable)obj);
                }
            }

            foreach (RenderableObject obj in FloorMiddle.ObjectsToRender)
            {
                if (obj is ICollidable)
                {
                    colliders.Add((ICollidable)obj);
                }
            }

            foreach (RenderableObject obj in FloorLeft.ObjectsToRender)
            {
                if (obj is ICollidable)
                {
                    colliders.Add((ICollidable)obj);
                }
            }

            return colliders;
        }

        internal void MoveFloorsUp(float diffTime)
        {
            FloorRight.MoveFloorUp(FloorOffsetPerSecond*diffTime);
            FloorMiddle.MoveFloorUp(FloorOffsetPerSecond * diffTime);
            FloorLeft.MoveFloorUp(FloorOffsetPerSecond * diffTime);
        }
    }
}
