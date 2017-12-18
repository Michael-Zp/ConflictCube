using System;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;
using OpenTK;

namespace ConflictCube.ComponentBased
{
    public static class FloorLoader
    {
        public static Floor Instance(string levelData, string floorName, Transform areaOfFloor, GameObject parent, CollisionGroup group)
        {
            GameObjectType[,] FloorTiles = GetFloorDataFromLevelfile(levelData, out int rows, out int columns);

            return LoadFloor(rows, columns, FloorTiles, floorName, areaOfFloor, parent, group);
        }

        private static Floor LoadFloor(int levelRows, int levelColumns, GameObjectType[,] floorTiles, string name, Transform areaOfFloor, GameObject parent, CollisionGroup group)
        {
            Vector2 floorTileSize = new Vector2(areaOfFloor.Size.X / levelColumns, 0.2f);
            Floor floorOfLevel = new Floor(name, areaOfFloor, parent, levelRows, levelColumns, group, floorTileSize);

            for (int row = 0; row < levelRows; row++)
            {
                for (int column = 0; column < levelColumns; column++)
                {
                    Transform tileTransform = floorOfLevel.BoxInFloorGrid(row, column);
                    
                    string tileName = "FloorTile, " + floorTiles.ToString() + " row: " + row + " column: " + column;
                    FloorTile floorTile = new FloorTile(row, column, tileName, tileTransform, parent, floorTiles[row, column], floorOfLevel);


                    floorOfLevel.AddFloorTile(floorTile, row, column);
                }
            }

            return floorOfLevel;
        }

        

        private static GameObjectType[,] GetFloorDataFromLevelfile(string levelData, out int levelRows, out int levelColumns)
        {
            GameObjectType[] FloorNumberToType = { GameObjectType.Finish, GameObjectType.Floor, GameObjectType.Hole, GameObjectType.Wall };

            levelData = levelData.Trim('\n');
            string[] allLines = levelData.Split('\n');
            levelRows = allLines.Length;
            levelColumns = allLines[0].Split(',').Length;

            GameObjectType[,] ret = new GameObjectType[levelRows, levelColumns];


            for (int i = 0; i < levelRows; i++)
            {
                string[] currentLine = allLines[i].Split(',');
                for (int u = 0; u < levelColumns; u++)
                {
                    try
                    {
                        int typeNumber = int.Parse(currentLine[u]);
                        
                        try
                        {
                            ret[i, u] = FloorNumberToType[typeNumber];
                        }
                        catch(Exception)
                        {
                            Console.WriteLine("TypeNumber " + typeNumber + " has no corrisponding type in the FloorNumberToTypeArray - FloorLoader.cs");
                            ret[i, u] = GameObjectType.Floor;
                        }
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("Could not parse string " + currentLine[u] + " to an integer - FloorLoader.cs");
                        ret[i, u] = GameObjectType.Floor;
                    }
                }
            }

            return ret;
        }
    }
}
