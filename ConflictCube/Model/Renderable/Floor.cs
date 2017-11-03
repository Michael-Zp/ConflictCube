using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictCube.Model.Renderable
{
    public class Floor : RenderableLayer
    {
        //Static 
        public static RenderLayerType FloorLayer { get; private set; }

        static Floor()
        {
            FloorLayer = RenderLayerType.Floor;
        }


        //Non - static
        public Vector2 FloorTileSize;
        private Vector2 _FloorSize;
        public Vector2 FloorSize {
            get {
                return _FloorSize;
            }
            set {
                _FloorSize = value;
                FloorTileSize.X = 2 / _FloorSize.X;
                FloorTileSize.Y = 2 / _FloorSize.Y;
            }
        }
        public TileType[,] FloorTypes { get; set; }
        public Dictionary<TileType, Tile> Tileset { get; private set; }


        public Floor(Vector2 floorSize, TileType[,] floorTypes, Dictionary<TileType, Tile> tileset) : base(new List<RenderableObject>())
        {
            FloorSize = floorSize;
            FloorTypes = floorTypes;
            Tileset = tileset;
        }

    }
}
