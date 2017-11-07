using ConflictCube.Model.Renderable;

namespace ConflictCube
{
    public class LevelBuilder
    {
        private static string LevelDirectoryPath = ".\\ConflictCube\\Levels\\";


        public static Level LoadLevel(int levelNumber)
        {
            Level newLevel = new Level();
            string levelPath = LevelDirectoryPath + "Level" + levelNumber + ".csv";

            newLevel.Floor = FloorLoader.Instance(levelPath);

            return newLevel;
        }

    }
}