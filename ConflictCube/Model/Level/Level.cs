using ConflictCube.Model;
using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ConflictCube
{
    public class Level : RenderableLayer
    {
        public Box2D AreaOfFloor { get; set; }
        public Floor FloorRight { get; set; }
        public Floor FloorMiddle { get; set; }
        public Floor FloorLeft { get; set; }
        public float FloorOffsetPerSecond { get; set; }
        public float StartRollingLevelOffsetSeconds { get; set; }

        private float ElapsedTimeInLevel = 0;


        public List<ICollidable> GetColliders()
        {
            List<ICollidable> colliders = new List<ICollidable>();

            foreach(RenderableObject obj in ObjectsToRender)
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

            UpdateRenderableObjects();
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

        private void UpdateRenderableObjects()
        {
            ObjectsToRender.Clear();
            ObjectsToRender.AddRange(FloorLeft.GetRenderableObjects());
            ObjectsToRender.AddRange(FloorMiddle.GetRenderableObjects());
            ObjectsToRender.AddRange(FloorRight.GetRenderableObjects());

            Matrix3 scaleMatrix = GenerateScaleMatrix();
            
            foreach(FloorTile obj in ObjectsToRender)
            {
                obj.Box = FloorCoordinatesToGlobalCoordinates(obj.Box, scaleMatrix);
            }
        }

        //TODO: Optimization use it as a memeber in the Floor class -> Dont calc it every frame
        private Matrix3 GenerateScaleMatrix()
        {
            float sizeRatioRows = AreaOfFloor.SizeX / (FloorRight.FloorBox.MaxX - FloorLeft.FloorBox.MinX);
            float sizeRatioColumns = AreaOfFloor.SizeY / 2;

            return Matrix3.CreateScale(sizeRatioRows, sizeRatioColumns, 1);
        }

        private Box2D FloorCoordinatesToGlobalCoordinates(Box2D box,  Matrix3 scaleMatrix)
        {
            Box2D newBox = new Box2D(box);

            //Scale
            Vector3 newSize = Vector3.Transform(new Vector3(newBox.SizeX, newBox.SizeY, 1), scaleMatrix);
            newBox.SizeX = newSize.X;
            newBox.SizeY = newSize.Y;

            //Transform
            newBox.CenterX = newBox.CenterX * scaleMatrix.Column0[0] + (AreaOfFloor.CenterX);
            newBox.CenterY = newBox.CenterY * scaleMatrix.Column1[1] + (AreaOfFloor.CenterY);

            return newBox;
        }

        public Vector2 FindStartPosition(Floor floor)
        {
            Matrix3 scaleMatrix = GenerateScaleMatrix();
            Vector2 pos = floor.FindStartPosition();
            Box2D posBox = FloorCoordinatesToGlobalCoordinates(new Box2D(pos.X, pos.Y, 0, 0), scaleMatrix);
            return new Vector2(posBox.CenterX, posBox.CenterY);
        }
        
    }
}
