using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictCube.Model
{
    interface IInputManager
    {
        void CloseGame();
        void MovePlayerLeft();
        void MovePlayerRight();
        void MovePlayerUp();
        void MovePlayerDown();
    }
}
