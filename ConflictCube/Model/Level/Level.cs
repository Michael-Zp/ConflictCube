using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using System.Collections.Generic;

namespace ConflictCube
{
    public class Level
    {
        public Floor Floor { get; set; }
        public float FloorOffsetPerSecond { get; set; }


        public List<ICollidable> GetColliders()
        {
            List<ICollidable> colliders = new List<ICollidable>();

            foreach(RenderableObject obj in Floor.ObjectsToRender)
            {
                if (obj is ICollidable)
                {
                    colliders.Add((ICollidable)obj);
                }
            }

            return colliders;
        }
    }
}
