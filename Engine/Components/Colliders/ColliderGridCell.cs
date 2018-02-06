using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine.Components
{
    public class ColliderGridCell : IEnumerable<string>
    {
        public string BottomLeft  => Cells[0];
        public string BottomRight => Cells[1];
        public string TopRight    => Cells[2];
        public string TopLeft     => Cells[3];

        public readonly int MinXCoord;
        public readonly int MaxXCoord;
        public readonly int MinYCoord;
        public readonly int MaxYCoord;

        private readonly List<string> Cells;

        public static ColliderGridCell GetGridCells(Collider collider, Vector2 cellSize)
        {
            return new ColliderGridCell(collider, cellSize);
        }

        private ColliderGridCell(Collider collider, Vector2 cellSize)
        {
            MinXCoord = (int)Math.Ceiling(collider.MinX / cellSize.X);
            MaxXCoord = (int)Math.Ceiling(collider.MaxX / cellSize.X);
            MinYCoord = (int)Math.Ceiling(collider.MinY / cellSize.Y);
            MaxYCoord = (int)Math.Ceiling(collider.MaxY / cellSize.Y);


            Cells = new List<string>
            {
                GetCoordString(MinXCoord, MinYCoord), //BottomLeft
                GetCoordString(MaxXCoord, MinYCoord), //BottomRight
                GetCoordString(MaxXCoord, MaxYCoord), //TopRight
                GetCoordString(MinXCoord, MaxYCoord) //TopLeft
            };
        }

        public static string GetCoordString(int xCoord, int yCoord)
        {
            return xCoord + "; " + yCoord;
        }


        public IEnumerator<string> GetEnumerator()
        {
            return Cells.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Cells.GetEnumerator();
        }
    }
}
