using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using System.Collections.Generic;

namespace ConflictCube
{
    public class Level
    {
        public Floor FloorRight { get; set; }
        public Floor FloorMiddle { get; set; }
        public Floor FloorLeft { get; set; }
        public float FloorOffsetPerSecond { get; set; }
        public float StartRollingLevelOffsetSeconds { get; set; }

        private float ElapsedTimeInLevel = 0;


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

        public void UpdateLevel(float diffTime)
        {
            ElapsedTimeInLevel += diffTime;

            if (ElapsedTimeInLevel >= StartRollingLevelOffsetSeconds)
            {
                MoveFloorsUp(diffTime);
            }
        }

        private void MoveFloorsUp(float diffTime)
        {
            MoveFloorUp(FloorLeft, diffTime);
            MoveFloorUp(FloorMiddle, diffTime);
            MoveFloorUp(FloorRight, diffTime);
        }

        private void MoveFloorUp(Floor floor, float diffTime)
        {
            floor.MoveFloorUp(FloorOffsetPerSecond * diffTime);
        }
    }
}
