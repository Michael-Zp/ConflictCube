using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using OpenTK;
using System;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ConflictCube
{
    public enum FloorArea
    {
        Left,
        Middle,
        Right
    }

    public class Level : RenderableLayer 
    {
        public Floor FloorRight { get; private set; }
        public Floor FloorMiddle { get; private set; }
        public Floor FloorLeft { get; private set; }
        public float FloorOffsetPerSecond { get; set; }
        public float StartRollingLevelOffsetSeconds { get; set; }

        private float ElapsedTimeInLevel = 0;
        
        private Boundary[] Boundaries =
        {
            new Boundary(new Box2D(-1.5f, -1f,  .5f,  2f), CollisionType.LeftBoundary),
            new Boundary(new Box2D( 1f,   -1f,  .5f,  2f), CollisionType.RightBoundary),
            new Boundary(new Box2D(-1f,    1f,   2f, .5f), CollisionType.TopBoundary),
        };

        public Level(Box2D areaOfLayer) : base(new List<RenderableObject>(), new List<RenderableLayer>(), areaOfLayer)
        {

        }

        public void AddFloor(FloorArea area, Floor floor)
        {
            switch(area)
            {
                case FloorArea.Left:
                    FloorLeft = floor;
                    break;

                case FloorArea.Middle:
                    FloorMiddle = floor;
                    break;

                case FloorArea.Right:
                    FloorRight = floor;
                    break;
            }

            SubLayers.Add(floor);
        }

        protected override List<ICollidable> GetAdditionalColliders()
        {
            List<ICollidable> colliders = new List<ICollidable>();

            foreach (Boundary boundary in Boundaries)
            {
                Vector2 newSize = TransformSizeToParent(boundary.CollisionBox.SizeX, boundary.CollisionBox.SizeY);
                Vector2 newPos = TransformSizeToParent(boundary.CollisionBox.MinX, boundary.CollisionBox.MinY);

                Boundary clone = boundary.Clone();

                clone.CollisionBox.SizeX = newSize.X;
                clone.CollisionBox.SizeY = newSize.Y;

                clone.CollisionBox.MinX = newPos.X;
                clone.CollisionBox.MinY = newPos.Y;

                colliders.Add(clone);
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

        public Vector2 FindStartPosition(FloorArea floor)
        {
            Vector2 pos;
            switch(floor)
            {
                case FloorArea.Left:
                    pos = FloorLeft.FindStartPosition();
                    break;

                case FloorArea.Middle:
                    pos = FloorMiddle.FindStartPosition();
                    break;

                case FloorArea.Right:
                    pos = FloorRight.FindStartPosition();
                    break;

                default:
                    throw new Exception("New floor area added without adding a start position switch case pair in the level");
            }

            return TransformPointToParent(pos);
        }
        
    }
}
