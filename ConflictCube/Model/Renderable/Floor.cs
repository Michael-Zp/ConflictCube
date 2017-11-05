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
        public FloorTile[,] FloorTiles { get; set; }
        public Dictionary<TileType, Tile> Tileset { get; private set; }

        public Floor(Vector2 floorSize, Dictionary<TileType, Tile> floorTileset) : base(new List<RenderableObject>())
        {
            FloorTiles = new FloorTile[(int)floorSize.Y, (int)floorSize.X];
            FloorSize = floorSize;
            Tileset = floorTileset;
        }

        public void MoveFloorUp(float distance)
        {
            foreach (FloorTile floorTile in FloorTiles)
            {
                floorTile.Box.MinY -= distance;
            }
        }
        

        public void AddFloorTile(FloorTile floorTile, int y, int x)
        {
            FloorTiles[y, x] = floorTile;
            ObjectsToRender.Add(floorTile);
        }
    }
}
