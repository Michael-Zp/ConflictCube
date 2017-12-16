using ConflictCube.ComponentBased.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class Pickable : GameObject
    {
        public Pickable(string name, Transform transform, GameObject parent, BoxCollider collider, Material material) : base(name, transform, parent)
        {
            AddComponent(collider);
            AddComponent(material);
        }
    }
}
