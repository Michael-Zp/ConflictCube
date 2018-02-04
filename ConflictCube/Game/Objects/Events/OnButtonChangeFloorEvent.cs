using System;
using System.Collections.Generic;

namespace ConflictCube.Objects
{
    public class OnButtonChangeFloorEvent : Event
    {
        public Floor Floor;
        public Dictionary<Tuple<int, int>, string> ChangesOnFloor;

        public OnButtonChangeFloorEvent(Floor floor)
        {
            Floor = floor;
            ChangesOnFloor = new Dictionary<Tuple<int, int>, string>();
        }

        public OnButtonChangeFloorEvent(Floor floor, Dictionary<Tuple<int, int>, string> changesOnFloor) : this(floor)
        {
            ChangesOnFloor = changesOnFloor;
        }

        public void AddChangeOnFloor(int row, int column, string type)
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
                ChangesOnFloor.TryGetValue(key, out string type);

                Floor.FloorTiles[key.Item1, key.Item2].ChangeFloorTile(type);
            }
        }
    }
}
