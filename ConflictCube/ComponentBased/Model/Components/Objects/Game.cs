using ConflictCube.ComponentBased.Components;
using System;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class Game : GameObject
    {
        public Game(string name, Transform transform, GameObject parent) : base(name, transform, parent, GameObjectType.Game)
        {

        }

        public override void OnUpdate()
        {
            if(Input.OnButtonIsPressed(InputKey.ExitApplication))
            {
                Environment.Exit(0);
            }

            List<GameObject> allFloors = FindGameObjectsByTypeInChildren<Floor>();

            bool floorShouldBreakDown = false;

            foreach (GameObject floor in allFloors)
            {
                floorShouldBreakDown = floorShouldBreakDown || ((Floor)floor).PlayerIsOverThreshold;
            }

            foreach (GameObject floor in allFloors)
            {
                ((Floor)floor).FloorShouldBreakDown = floorShouldBreakDown;
            }
        }
    }
}
