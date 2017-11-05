using OpenTK;
using ConflictCube.Model.Renderable;
using Zenseless.OpenGL;
using System;
using System.Xml;
using System.Collections.Generic;

namespace ConflictCube.Model.Tiles
{
    public enum TileType
    {
        Floor,
        Wall,
        Hole,
        Finish,
        Player
    }

    public abstract class TileTypeBase
    {
        // With this construct tilesets can be loaded 
        public static TileType GetTypeOfTileNumber<T>(int tileNumber) where T : new()
        {
            T newT = new T();
            if (newT is FloorTileType)
            {
                return FloorTileType.GetTypeOfTileNumber(tileNumber);
            }
            else if (newT is PlayerTileType)
            {
                return PlayerTileType.GetTypeOfTileNumber(tileNumber);
            }
            
            throw new NotImplementedException();
        }
    }

    public class FloorTileType : TileTypeBase
    {
        private static TileType[] FloorNumberToType = { TileType.Finish, TileType.Floor, TileType.Hole, TileType.Wall };

        public static TileType GetTypeOfTileNumber(int tileNumber)
        {
            return FloorNumberToType[tileNumber];
        }
    }

    public class PlayerTileType : TileTypeBase
    {
        private static TileType[] TileNumberToType = { TileType.Player };

        public static TileType GetTypeOfTileNumber(int tileNumber)
        {
            return TileNumberToType[tileNumber];
        }
    }


    public class TilesetTile
    {
        public TileType Type { get; private set; }
        public Texture Texture { get; private set; }


        public TilesetTile(TileType type, Texture texture)
        {
            Type = type;
            Texture = texture;
        }
    }


    public class FloorTile : RenderableObject
    {
        public TileType Type { get; private set; }

        public FloorTile(TilesetTile tile, Vector2 size, Vector2 position) : base(position, size, tile.Texture)
        {
            if (tile.Type != TileType.Finish &&
                tile.Type != TileType.Floor &&
                tile.Type != TileType.Hole &&
                tile.Type != TileType.Wall)
            {
                throw new System.Exception("FloorTile was initalized with wrong TileType");
            }

            Type = tile.Type;
        }
    }

    public class PlayerTile : RenderableObject
    {
        public TileType Type { get; private set; }

        public PlayerTile(TilesetTile tile, Vector2 size, Vector2 position) : base(position, size, tile.Texture)
        {
            if (tile.Type != TileType.Player )
            {
                throw new System.Exception("PlayerTile was initalized with wrong TileType");
            }

            Type = tile.Type;
        }
    }
}
