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

            newLevel.FloorRight = FloorLoader.Instance(levelPathRight,   new Box2D( 1f / 21f, -1f, 10f / 21f, 1f));
            newLevel.FloorMiddle = FloorLoader.Instance(levelPathMiddle, new Box2D(-1f / 21f, -1f,  1f / 21f, 1f));
            newLevel.FloorLeft = FloorLoader.Instance(levelPathLeft,     new Box2D(-1f      , -1f, 10f / 21f, 1f));

            return newLevel;
        }

    }
}