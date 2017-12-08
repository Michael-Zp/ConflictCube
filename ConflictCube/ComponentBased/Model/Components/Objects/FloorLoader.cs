using System;
using System.Collections.Generic;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;

namespace ConflictCube.ComponentBased
{
    public static class FloorLoader
    {
        public static Floor Instance(string levelData, string floorName, Transform areaOfFloor, GameObject parent, Dictionary<GameObjectType, Material> materials)
        {
            GameObjectType[,] FloorTiles = GetFloorDataFromLevelfile(levelData, out int rows, out int columns);

            return LoadFloor(rows, columns, FloorTiles, floorName, areaOfFloor, parent, materials);
        }

        private static Floor LoadFloor(int levelRows, int levelColumns, GameObjectType[,] floorTiles, string name, Transform areaOfFloor, GameObject parent, Dictionary<GameObjectType, Material> materials)
        {
            Floor floorOfLevel = new Floor(name, areaOfFloor, parent, levelRows, levelColumns);

            for (int row = 0; row < levelRows; row++)
            {
                for (int column = 0; column < levelColumns; column++)
                {
                    Transform tileTransform = floorOfLevel.BoxInFloorGrid(row, column);

                    Material material;
                    try
                    {
                        materials.TryGetValue(floorTiles[row, column], out material);
                    }
                    catch(Exception)
                    {
                        throw new Exception("Did not found material for floor type: " + floorTiles[row, column].ToString());
                    }

                    string tileName = "FloorTile, " + floorTiles.ToString() + " row: " + row + " column: " + column;
                    FloorTile floorTile = new FloorTile(row, column, tileName, tileTransform, material, parent, floorTiles[row, column]);

                    if(floorTiles[row, column] == GameObjectType.Wall)
                    {
                        floorTile.AddComponent(new BoxCollider(tileTransform, false, null));
                    }
                    else if(floorTiles[row, column] == GameObjectType.Hole)
                    {
                        floorTile.AddComponent(new BoxCollider(new Transform(tileTransform.Position.X, tileTransform.Position.Y, tileTransform.Size.X * .8f, tileTransform.Size.Y * .8f), false, null));
                    }

                    floorOfLevel.AddFloorTile(floorTile, row, column);
                }
            }

            return floorOfLevel;
        }

        

        private static GameObjectType[,] GetFloorDataFromLevelfile(string levelPath, out int levelRows, out int levelColumns)
        {
            GameObjectType[] FloorNumberToType = { GameObjectType.Finish, GameObjectType.Floor, GameObjectType.Hole, GameObjectType.Wall };

            string[] allLines = levelPath.Split('\n');
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
