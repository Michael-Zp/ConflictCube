using ConflictCube.Model.Renderable;
using Zenseless.Geometry;

namespace ConflictCube
{
    public class LevelBuilder
    {
        private static string LevelDirectoryPath = ".\\ConflictCube\\Levels\\";


        public static Level LoadLevel(int levelNumber)
        {
            Level newLevel = new Level(new Box2D(-.8f, -1f, 1.6f, 6f));
            string levelPathLeft = LevelDirectoryPath + "Level" + levelNumber + "Left.csv";
            string levelPathMiddle = LevelDirectoryPath + "Level" + levelNumber + "Middle.csv";
            string levelPathRight = LevelDirectoryPath + "Level" + levelNumber + "Right.csv";

            newLevel.AddFloor(FloorArea.Left,   FloorLoader.Instance(levelPathLeft,   new Box2D(-1f      , -1f, 20f / 21f, 2f)));
            newLevel.AddFloor(FloorArea.Middle, FloorLoader.Instance(levelPathMiddle, new Box2D(-1f / 21f, -1f,  2f / 21f, 2f)));
            newLevel.AddFloor(FloorArea.Right,  FloorLoader.Instance(levelPathRight,  new Box2D( 1f / 21f, -1f, 20f / 21f, 2f)));
            

            return newLevel;
        }

    }
}