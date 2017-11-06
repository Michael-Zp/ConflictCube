using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictCube.Model
{
    public enum CollisionType
    {
        LeftBoundary,
        RightBoundary,
        TopBoundary,
        BottomBoundary,
        Player
    }

    public interface ICollidable
    {
        CollisionType CollisionType { get; }

        void OnCollide(CollisionType type);
    }
}
