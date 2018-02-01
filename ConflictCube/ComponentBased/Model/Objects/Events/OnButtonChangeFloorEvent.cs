using System;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Model.Components.Objects.Events
{
    public class OnButtonChangeFloorEvent : Event
    {
        public Floor Floor;
        public Dictionary<Tuple<int, int>, GameObjectType> ChangesOnFloor;

        public OnButtonChangeFloorEvent(Floor floor)
        {
            Floor = floor;
            ChangesOnFloor = new Dictionary<Tuple<int, int>, GameObjectType>();
        }

        public OnButtonChangeFloorEvent(Floor floor, Dictionary<Tuple<int, int>, GameObjectType> changesOnFloor) : this(floor)
        {
            ChangesOnFloor = changesOnFloor;
        }

        public void AddChangeOnFloor(int row, int column, GameObjectType type)
        {
            ChangesOnFloor.Add(Tuple.Create(row, column), type);
        }

        public override void StartEvent()
        {
            if (IsStarted)
                return;

            IsStarted = true;

            foreach(Tuple<int, int> key in ChangesOnFloor.Keys)
            {
                ChangesOnFloor.TryGetValue(key, out GameObjectType type);

                Floor.FloorTiles[key.Item1, key.Item2].ChangeFloorTile(type);
            }
        }
    }
}
