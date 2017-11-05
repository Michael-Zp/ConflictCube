using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Zenseless.OpenGL;

namespace ConflictCube
{
    public class LevelBuilder
    {
        private static string LevelDirectoryPath = ".\\ConflictCube\\Levels\\";
        private static string FloorTilesetDescriptionPath = LevelDirectoryPath + "Tileset.tsx";
        private static string FloorTilesetPngPath = LevelDirectoryPath + "Tileset.png";

        public static Tileset<FloorTileType> FloorTileset;

        
        static LevelBuilder()
        {
            LoadFloorTileset();
        }

        private static void LoadFloorTileset()
        {
            FloorTileset = new Tileset<FloorTileType>(FloorTilesetDescriptionPath, FloorTilesetPngPath);
        }

        public static Level LoadLevel(int levelNumber)
        {
            Level newLevel = new Level();
            string levelPath = LevelDirectoryPath + "Level" + levelNumber + ".csv";

            newLevel.Floor = Floor.Instance(levelPath, FloorTileset);

            return newLevel;
        }

    }
}