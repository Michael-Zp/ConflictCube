using OpenTK;
using ConflictCube.Model.Renderable;
using Zenseless.OpenGL;
using System;
using System.Xml;
using System.Collections.Generic;
using Zenseless.Geometry;

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

    public enum TilesetType
    {
        Floor,
        Player
    }

    public static class TileTypeBase
    {
        public static string TilesetPath = ".\\ConflictCube\\Levels\\";

        // With this construct tilesets can be loaded 
        public static TileType GetTypeOfTileNumber(TilesetType type, int tileNumber)
        {
            switch(type)
            {
                case TilesetType.Floor:
                    return FloorTileType.GetTypeOfTileNumber(tileNumber);

                case TilesetType.Player:
                    return PlayerTileType.GetTypeOfTileNumber(tileNumber);
            }
            
            throw new NotImplementedException();
        }

        public static string GetTilesetDescriptionPath(TilesetType type)
        {
            switch (type)
            {
                case TilesetType.Floor:
                    return FloorTileType.FloorTilesetDescriptionPath;

                case TilesetType.Player:
                    return PlayerTileType.PlayerTilesetDescriptionPath;
            }

            throw new NotImplementedException();
        }

        public static string GetTilesetPngPath(TilesetType type)
        {
            switch (type)
            {
                case TilesetType.Floor:
                    return FloorTileType.FloorTilesetPngPath;

                case TilesetType.Player:
                    return PlayerTileType.PlayerTilesetPngPath;
            }

            throw new NotImplementedException();
        }
    }

    public static class FloorTileType
    {
        public static string FloorTilesetDescriptionPath = TileTypeBase.TilesetPath + "Tileset.tsx";
        public static string FloorTilesetPngPath = TileTypeBase.TilesetPath + "Tileset.png";

        private static TileType[] FloorNumberToType = { TileType.Finish, TileType.Floor, TileType.Hole, TileType.Wall };

        public static TileType GetTypeOfTileNumber(int tileNumber)
        {
            return FloorNumberToType[tileNumber];
        }
    }

    public static class PlayerTileType
    {
        public static string PlayerTilesetDescriptionPath = TileTypeBase.TilesetPath + "Player.tsx";
        public static string PlayerTilesetPngPath = TileTypeBase.TilesetPath + "Player.gif";

        private static TileType[] TileNumberToType = { TileType.Player };

        public static TileType GetTypeOfTileNumber(int tileNumber)
        {
            return TileNumberToType[tileNumber];
        }
    }


    public class FloorTile : RenderableObject, ICollidable
    {
        public int Row { get; private set; }
        public int Column { get; private set; }

        public CollisionType CollisionType { get; }

        public FloorTile(TileType type, Box2D box, int row, int column) : base(box, type)
        {

            switch(type)
            {
                case TileType.Finish:
                    CollisionType = CollisionType.Finish;
                    break;

                case TileType.Hole:
                    CollisionType = CollisionType.Hole;
                    break;

                case TileType.Wall:
                    CollisionType = CollisionType.Wall;
                    break;

                case TileType.Floor:
                    CollisionType = CollisionType.NoCollision;
                    break;

                default:
                    throw new System.Exception("FloorTile was initalized with wrong TileType");
            }
            
            Row = row;
            Column = column;
        }

        public void OnCollide(CollisionType type)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerTile : RenderableObject
    {
        public PlayerTile(TileType type, Vector2 size, Vector2 position) : base(position, size, type)
        {
            if (type != TileType.Player )
            {
                throw new System.Exception("PlayerTile was initalized with wrong TileType");
            }
        }
    }
}
