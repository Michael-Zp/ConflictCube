using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Zenseless.HLGL;

namespace ConflictCube
{
    public class Level
    {
        public int[,] LevelTiles { get; private set; }
        public Dictionary<int, LevelBuilder.Tile> Tileset { get; private set; }

        public Level(int[,] levelTiles, Dictionary<int, LevelBuilder.Tile> tileset)
        {
            LevelTiles = levelTiles;
            Tileset = tileset;
        }
    }
}
