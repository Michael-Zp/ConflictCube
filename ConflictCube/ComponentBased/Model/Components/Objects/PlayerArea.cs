using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class PlayerArea : GameObject
    {
        public PlayerArea(string name, Transform transform, GameObject parent) : base(name, transform, parent)
        {
        }

        public override void OnUpdate()
        {
            Player player = (Player)FindGameObjectByTypeInChildren<Player>();

            if(player != null)
            {
                Transform globalPlayerPosition = player.Transform.TransformToGlobal();
                Transform localPlayerPosition = Transform.TransformToLocal(globalPlayerPosition);
                //does not work
                //Transform.Position = new OpenTK.Vector2(Transform.Position.X, -localPlayerPosition.Position.Y);
            }
            base.OnUpdate();
        }
    }
}
