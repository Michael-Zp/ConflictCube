using System;
using OpenTK;
using NUnit.Framework;
using ConflictCube.ComponentBased.Components;

namespace ConflictCubeTest
{
    [TestFixture]
    public class UnitTest1
    {

        private void SetupOpenTK()
        {
            GameWindow window = new GameWindow();
        }


        [Test]
        public void TestTransformLocalToGlobalToLocal()
        {
            Transform localTransform = new Transform(0, 1, 2f, 2f);

            Transform globalTransform = new Transform(0, 0, 1, 1);

            GameObject globalObject = new GameObject("global", globalTransform);
            GameObject localObject = new GameObject("local", localTransform);

            globalObject.AddChild(localObject);

            Vector2 globalPosition = localTransform.TransformToLocal(globalTransform).Position;

            Assert.AreEqual(globalPosition, new Vector2(0, -0.5f));
        }

        /*
        [Test]
        [Category("OpenGlTests")]
        public void TestLoadLevel()
        {
            SetupOpenTK();

            Level level = LevelBuilder.LoadLevel(0);

            int[,] level0 = {
                                {0,0,0,0},
                                {1,1,1,1},
                                {1,3,3,1},
                                {1,1,3,3},
                                {2,1,1,3},
                                {3,3,1,1},
                                {3,3,1,2},
                                {3,3,1,1},
                                {3,1,1,1},
                                {3,1,3,3}
            };

            TileType[,] ftLevel0 = new TileType[10, 4];

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    ftLevel0[y, x] = FloorTileType.GetTypeOfTileNumber(level0[y, x]);
                }
            }

            TileType[,] loadedLevel0 = new TileType[10, 4];

            Assert.AreEqual(loadedLevel0.GetLength(0), level.FloorLeft.FloorTiles.GetLength(0));
            Assert.AreEqual(loadedLevel0.GetLength(1), level.FloorLeft.FloorTiles.GetLength(1));

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    loadedLevel0[y, x] = level.FloorLeft.FloorTiles[y, x].Type;
                }
            }

            
            Assert.AreEqual(ftLevel0, loadedLevel0);
        }

        [Test]
        [Category("OpenGlTests")]
        public void CreateNewTexture()
        {
            SetupOpenTK();

            int texture;
            try
            {
                texture = OpenTK.Graphics.OpenGL4.GL.GenTexture();
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        
        [Test]
        [Category("OpenGlTests")]
        public void SetStartPosition()
        {
            SetupOpenTK();

            Level level = LevelBuilder.LoadLevel(0);

            Vector2 startPosition = level.FloorLeft.FindStartPosition();
            Vector2 shouldStartPosition = new Vector2(-1 + (15f / 42.0f), -1 + 1.0f / 10.0f);

            AssertVectorsAreRoughlyTheSame(shouldStartPosition, startPosition);


            //One tile above the start field is another floor field -> Move one up should not change the position
            level.FloorLeft.MoveFloorUp(level.FloorLeft.FloorTileSize.Y);

            startPosition = level.FloorLeft.FindStartPosition();
            shouldStartPosition = new Vector2(-1 + (15f / 42.0f), -1 + 1.0f / 10.0f);

            AssertVectorsAreRoughlyTheSame(shouldStartPosition, startPosition);

            //Now the start field is one tile to the right
            level.FloorLeft.MoveFloorUp(level.FloorLeft.FloorTileSize.Y);
        
            startPosition = level.FloorLeft.FindStartPosition();
            shouldStartPosition = new Vector2(-1 + (25.0f / 42.0f), -1 + 1.0f / 10.0f);

            AssertVectorsAreRoughlyTheSame(shouldStartPosition, startPosition);
        }

        */

        [Test]
        public void PassingTest()
        {
            Assert.Pass();
        }


        private void AssertVectorsAreRoughlyTheSame(Vector2 one, Vector2 two, float epsilon = 0.0001f)
        {
            Assert.That(Math.Abs(one.X - two.X) < epsilon, one.X + " - " + two.X + " is bigger than epsilon of " + epsilon);
            Assert.That(Math.Abs(one.Y - two.Y) < epsilon, one.X + " - " + two.X + " is bigger than epsilon of " + epsilon);
        }
    }
}
