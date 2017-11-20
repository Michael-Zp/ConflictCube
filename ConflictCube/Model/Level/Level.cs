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

        public void UpdateLevel()
        {
            ElapsedTimeInLevel += Time.Time.DifTime;

            if (ElapsedTimeInLevel >= StartRollingLevelOffsetSeconds)
            {
                MoveFloorsUp(Time.Time.DifTime);
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

        /// <summary>
        ///     Returns the grid position with an offset. If applying the offset means switching from
        ///     one floor to another this is handeled correctly.
        ///     If no possition could be found (Going over the boundaries) throws an exception.
        /// </summary>
        /// <param name="originPos"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Tuple<Vector2, FloorArea> GetPositionInLevelWithOffset(Tuple<Vector2, FloorArea> originGridPosition, Vector2 offset)
        {
            Tuple<float, FloorArea> columnPosition;

            try
            {
                columnPosition = GetColumnAndFloorWithOffset(originGridPosition, offset.X);
            }
            catch (Exception e)
            {
                throw e;
            }

            float rowPosition = GetRowPositionWithFloorOffset(originGridPosition.Item2, columnPosition.Item2, originGridPosition.Item1.Y);

            Vector2 position = new Vector2(columnPosition.Item1, rowPosition + offset.Y);

            if(position.Y < 0 || position.Y >= FloorAreaToFloor(columnPosition.Item2).FloorSize.Y)
            {
                throw new Exception("Position in level is out of bounds of the rows");
            }

            return Tuple.Create(position, columnPosition.Item2);
        }

        private Tuple<float, FloorArea> GetColumnAndFloorWithOffset(Tuple<Vector2, FloorArea> currentPosition, float columnsOffset)
        {
            float columnPosition = currentPosition.Item1.X + columnsOffset;

            float globalColumn = columnPosition;

            switch (currentPosition.Item2)
            {
                case FloorArea.Middle:
                    globalColumn += FloorLeft.FloorSize.X;
                    break;

                case FloorArea.Right:
                    globalColumn += FloorLeft.FloorSize.X + FloorMiddle.FloorSize.X;
                    break;
            }

            FloorArea actualArea;

            if(globalColumn < 0 || globalColumn > (FloorLeft.FloorSize.X + FloorMiddle.FloorSize.X + FloorRight.FloorSize.X))
            {
                throw new Exception("Box with offset is not in a column of the floor.");
            }
            else if (globalColumn >= 0 && globalColumn < FloorLeft.FloorSize.X)
            {
                actualArea = FloorArea.Left;
            }
            else if (globalColumn < (FloorLeft.FloorSize.X + FloorMiddle.FloorSize.X))
            {
                actualArea = FloorArea.Middle;
                globalColumn -= FloorLeft.FloorSize.X;
            }
            else if (globalColumn < (FloorLeft.FloorSize.X + FloorMiddle.FloorSize.X + FloorRight.FloorSize.X))
            {
                actualArea = FloorArea.Right;
                globalColumn -= FloorLeft.FloorSize.X;
                globalColumn -= FloorMiddle.FloorSize.X;
            }
            else
            {
                throw new Exception("Something went terribly wrong");
            }

            return Tuple.Create(globalColumn, actualArea);
        }

        
        private float GetRowPositionWithFloorOffset(FloorArea originFloorArea, FloorArea floorWithOffset, float currentRow)
        {
            List<Tuple<FloorArea, FloorArea>> floorTransitions = GetFloorTransition(originFloorArea, floorWithOffset);

            foreach(Tuple<FloorArea, FloorArea> transition in floorTransitions)
            {
                Floor originFloor, targetFloor;

                originFloor = FloorAreaToFloor(transition.Item1);
                targetFloor = FloorAreaToFloor(transition.Item2);

                float originalToTargetFoorTileRow = targetFloor.FloorSize.Y / originFloor.FloorSize.Y;

                currentRow *= originalToTargetFoorTileRow;
            }

            return currentRow;
        }

        private List<Tuple<FloorArea, FloorArea>> GetFloorTransition(FloorArea originFloor, FloorArea actualFloor)
        {
            List<Tuple<FloorArea, FloorArea>> floorTransitions = new List<Tuple<FloorArea, FloorArea>>();

            switch(originFloor)
            {
                case FloorArea.Left:
                    switch (actualFloor)
                    {
                        case FloorArea.Middle:
                            floorTransitions.Add(Tuple.Create(FloorArea.Left, FloorArea.Middle));
                            break;

                        case FloorArea.Right:
                            floorTransitions.Add(Tuple.Create(FloorArea.Left, FloorArea.Middle));
                            floorTransitions.Add(Tuple.Create(FloorArea.Middle, FloorArea.Right));
                            break;
                    }
                    break;

                case FloorArea.Middle:
                    switch (actualFloor)
                    {
                        case FloorArea.Left:
                            floorTransitions.Add(Tuple.Create(FloorArea.Middle, FloorArea.Left));
                            break;

                        case FloorArea.Right:
                            floorTransitions.Add(Tuple.Create(FloorArea.Middle, FloorArea.Right));
                            break;
                    }
                    break;


                case FloorArea.Right:
                    switch (actualFloor)
                    {
                        case FloorArea.Left:
                            floorTransitions.Add(Tuple.Create(FloorArea.Right, FloorArea.Middle));
                            floorTransitions.Add(Tuple.Create(FloorArea.Middle, FloorArea.Left));
                            break;

                        case FloorArea.Middle:
                            floorTransitions.Add(Tuple.Create(FloorArea.Middle, FloorArea.Right));
                            break;
                    }
                    break;
            }

            return floorTransitions;
        }
        
        /// <summary>
        ///     Return the 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Tuple<Vector2, FloorArea> GetGridPosition(Vector2 position)
        {
            Vector2 localPlayerPos = TransformPointToLocal(position);
            Vector2 gridPosition;

            FloorArea area;
            
            if(FloorLeft.AreaOfLayer.Contains(localPlayerPos.X, localPlayerPos.Y))
            {
                area = FloorArea.Left;
                gridPosition = FloorLeft.GetGridPosition(localPlayerPos);
            }
            else if (FloorMiddle.AreaOfLayer.Contains(localPlayerPos.X, localPlayerPos.Y))
            {
                area = FloorArea.Middle;
                gridPosition = FloorLeft.GetGridPosition(localPlayerPos);
            }
            else if (FloorRight.AreaOfLayer.Contains(localPlayerPos.X, localPlayerPos.Y))
            {
                area = FloorArea.Right;
                gridPosition = FloorLeft.GetGridPosition(localPlayerPos);
            }
            else
            {
                throw new Exception("Player not in any floor area");
            }
            
            return Tuple.Create(gridPosition, area);
        }


        /// <summary>
        ///     Returns the Box2D in parent coordinates from the grid.
        ///     If no box could be found throws an exception.
        /// </summary>
        /// <param name="gridPosition">Orign grid pos</param>
        /// <param name="offset">Offset to this origin</param>
        /// <returns></returns>
        public Box2D GetBoxForGridOffsetOfPosition(Tuple<Vector2, FloorArea> gridPosition)
        {
            Vector2 boxGridPosition = gridPosition.Item1;
            Box2D boxInGrid;

            switch (gridPosition.Item2)
            {
                case FloorArea.Left:
                    boxInGrid = FloorLeft.GetBoxAtGridPosition(boxGridPosition);
                    break;

                case FloorArea.Middle:
                    boxInGrid = FloorMiddle.GetBoxAtGridPosition(boxGridPosition);
                    break;

                case FloorArea.Right:
                    boxInGrid = FloorRight.GetBoxAtGridPosition(boxGridPosition);
                    break;

                default:
                    throw new Exception("Box in grid not found from this gridPosition " + gridPosition + " .");
            }

            Vector2 globalMin = TransformPointToParent(boxInGrid.MinX, boxInGrid.MinY);
            Vector2 globalSize = TransformSizeToParent(boxInGrid.SizeX, boxInGrid.SizeY);

            return new Box2D(globalMin.X, globalMin.Y, globalSize.X, globalSize.Y);
        }


        private Floor FloorAreaToFloor(FloorArea area)
        {
            switch(area)
            {
                case FloorArea.Left:
                    return FloorLeft;

                case FloorArea.Middle:
                    return FloorMiddle;

                case FloorArea.Right:
                    return FloorRight;

                default:
                    throw new Exception("No floor found for FloorArea: " + area);
            }
        }
    }
}
