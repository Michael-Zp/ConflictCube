using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.Model.Components.Scene
{
    public class Camera : GameObject
    {

        public float YDistanceOfPlayer = 0;

        public Camera(string name, Transform transform) : base(name, transform)
        {

        }
    }
}
