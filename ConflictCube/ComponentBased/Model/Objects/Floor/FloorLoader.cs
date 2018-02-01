using System;
using System.Collections.Generic;
using System.Xml;
using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;
using ConflictCube.ComponentBased.Model.Components.Objects.Events;
using OpenTK;

namespace ConflictCube.ComponentBased
{
    public static class FloorLoader
    {
        public static Floor Instance(string levelData, string floorName, Transform areaOfFloor, CollisionGroup group, GameObject parent)
        {
            List<int[,]> FloorTiles = ReadLayersOfLevel(levelData, out int rows, out int columns);

            return LoadFloor(rows, columns, FloorTiles, floorName, areaOfFloor, group, parent);
        }

        private static Floor LoadFloor(int levelRows, int levelColumns, List<int[,]> floorTiles, string name, Transform floorTransform, CollisionGroup group, GameObject parent)
        {
            Vector2 floorTileSize = new Vector2(0.13f, 0.13f);
            Floor floorOfLevel = new Floor(name, floorTransform, levelRows, levelColumns, group, floorTileSize, parent);

            for (int row = 0; row < levelRows; row++)
            {
                for (int column = 0; column < levelColumns; column++)
                {
                    Transform tileTransform = floorOfLevel.BoxInFloorGrid(row, column);

                    string tileName = "FloorTile, " + floorTiles.ToString() + " row: " + row + " column: " + column;

                    LevelTile floorTile = new LevelTile(row, column, tileName, tileTransform, floorTiles[0][row, column], floorOfLevel, floorOfLevel);
                    LevelTile buttonTile = new LevelTile(row, column, tileName, tileTransform, floorTiles[1][row, column], floorOfLevel, floorOfLevel);
                    LevelTile cubeTile = new LevelTile(row, column, tileName, tileTransform, floorTiles[2][row, column], floorOfLevel, floorOfLevel);

                    switch(floorTiles[3][row, column])
                    {
                        case 42:
                            floorOfLevel.BluePlayerCheckpoint = tileTransform;
                            floorOfLevel.OrangePlayerCheckpoint = tileTransform;
                            break;

                        case 43:
                            floorOfLevel.OrangePlayerCheckpoint = tileTransform;
                            break;

                        case 44:
                            floorOfLevel.BluePlayerCheckpoint = tileTransform;
                            break;
                    }

                    if (buttonTile.Type == GameObjectType.NotActiveButton)
                    {
                        buttonTile.Event = GenerateEvent(row, column, levelRows, levelColumns, floorTiles, floorOfLevel);
                    }


                    floorOfLevel.AddLevelTile(floorTile, buttonTile, cubeTile, row, column);
                }
            }

            return floorOfLevel;
        }

        private static OnButtonChangeFloorEvent GenerateEvent(int row, int column, int levelRows, int levelColumns, List<int[,]> floorTiles, Floor floor)
        {
            OnButtonChangeFloorEvent btnEvent = new OnButtonChangeFloorEvent(floor);

            for (int i = 3; i < floorTiles.Count; i++)
            {
                if (LevelTile.GetGameObjectTypeForIndex(floorTiles[i][row, column]) != GameObjectType.NotActiveButton)
                {
                    continue;
                }


                for (int tempRow = 0; tempRow < levelRows; tempRow++)
                {
                    for (int tempColumn = 0; tempColumn < levelColumns; tempColumn++)
                    {
                        if (row == tempRow && column == tempColumn)
                        {
                            continue;
                        }

                        GameObjectType type = LevelTile.GetGameObjectTypeForIndex(floorTiles[i][tempRow, tempColumn]);

                        if (type == GameObjectType.None)
                        {
                            continue;
                        }

                        btnEvent.AddChangeOnFloor(tempRow, tempColumn, type);
                    }
                }
            }

            return btnEvent;
        }


        private static List<int[,]> ReadLayersOfLevel(string levelData, out int levelRows, out int levelColumns)
        {
            List<int[,]> layersOfLevel = new List<int[,]>();

            XmlDocument level = new XmlDocument();
            level.LoadXml(levelData);

            XmlNodeList layersData = level.SelectNodes("/map/layer/data");

            levelRows = -1;
            levelColumns = -1;

            foreach (XmlNode layerData in layersData)
            {
                if (levelRows == -1 || levelColumns == -1)
                {
                    layersOfLevel.Add(GetFloorDataFromLevelfile(layerData.InnerText, out levelRows, out levelColumns));
                }
                else
                {
                    layersOfLevel.Add(GetFloorDataFromLevelfile(layerData.InnerText, levelRows, levelColumns));
                }
            }

            return layersOfLevel;
        }

        private static int[,] GetFloorDataFromLevelfile(string levelData, int levelRows, int levelColumns)
        {
            return GetFloorDataFromLevellines(GetLinesOfLevelData(levelData), levelRows, levelColumns);
        }

        private static int[,] GetFloorDataFromLevelfile(string levelData, out int levelRows, out int levelColumns)
        {
            string[] allLines = GetLinesOfLevelData(levelData);

            levelRows = allLines.Length;
            levelColumns = allLines[0].Split(',').Length;

            return GetFloorDataFromLevellines(allLines, levelRows, levelColumns);
        }

        private static int[,] GetFloorDataFromLevellines(string[] allLines, int levelRows, int levelColumns)
        {
            int[,] ret = new int[levelRows, levelColumns];


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
                            ret[i, u] = typeNumber;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("TypeNumber " + typeNumber + " has no corrisponding type in the FloorNumberToTypeArray - FloorLoader.cs");
                            ret[i, u] = 1;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not parse string " + currentLine[u] + " to an integer - FloorLoader.cs");
                        ret[i, u] = 1;
                    }
                }
            }

            return ret;
        }

        private static string[] GetLinesOfLevelData(string levelData)
        {
            levelData = levelData.Trim();

            string[] levelLines = levelData.Split('\n');

            for (int i = 0; i < levelLines.Length; i++)
            {
                levelLines[i] = levelLines[i].Trim().TrimEnd(',');
            }

            return levelLines;
        }
    }
}
