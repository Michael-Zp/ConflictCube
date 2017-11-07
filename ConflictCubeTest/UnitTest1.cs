using System;
using OpenTK;
using ConflictCube;
using NUnit.Framework;
using ConflictCube.Model.Tiles;

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

            Assert.AreEqual(loadedLevel0.GetLength(0), level.Floor.FloorTiles.GetLength(0));
            Assert.AreEqual(loadedLevel0.GetLength(1), level.Floor.FloorTiles.GetLength(1));

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    loadedLevel0[y, x] = level.Floor.FloorTiles[y, x].Type;
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

            Vector2 startPosition = level.Floor.FindStartPosition();

            Assert.AreEqual(new Vector2(-0.25f, -0.9f), startPosition);


            //One tile above the start field is another floor field -> Move one up should not change the position
            level.Floor.MoveFloorUp(level.Floor.FloorTileSize.Y);

            startPosition = level.Floor.FindStartPosition();

            Assert.AreEqual(new Vector2(-0.25f, -0.9f), startPosition);

            //Now the start field is one tile to the right
            level.Floor.MoveFloorUp(level.Floor.FloorTileSize.Y);

            startPosition = level.Floor.FindStartPosition();

            Assert.AreEqual(new Vector2(0.25f, -0.9f), startPosition);
        }

        [Test]
        public void PassingTest()
        {
            Assert.Pass();
        }
    }
}
