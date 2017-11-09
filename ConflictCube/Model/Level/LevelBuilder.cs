using ConflictCube.Model.Renderable;
using Zenseless.Geometry;

namespace ConflictCube
{
    public class LevelBuilder
    {
        private static string LevelDirectoryPath = ".\\ConflictCube\\Levels\\";


        public static Level LoadLevel(int levelNumber)
        {
            Level newLevel = new Level();
            string levelPathRight = LevelDirectoryPath + "Level" + levelNumber + "Right.csv";
            string levelPathMiddle = LevelDirectoryPath + "Level" + levelNumber + "Middle.csv";
            string levelPathLeft = LevelDirectoryPath + "Level" + levelNumber + "Left.csv";

            newLevel.FloorRight = FloorLoader.Instance(levelPathRight,   new Box2D( 1f / 21f, -1f, 20f / 21f, 2f));
            newLevel.FloorMiddle = FloorLoader.Instance(levelPathMiddle, new Box2D(-1f / 21f, -1f,  2f / 21f, 2f));
            newLevel.FloorLeft = FloorLoader.Instance(levelPathLeft,     new Box2D(-1f      , -1f, 20f / 21f, 2f));

            //Default is entire screen
            newLevel.AreaOfFloor = new Box2D(-.8f, -1, 1.6f, 2);

            return newLevel;
        }

    }
}