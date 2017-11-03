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
        public void TestLoadTileset()
        {
            SetupOpenTK();

            Assert.AreEqual(4, LevelBuilder.FloorTileset.Count);


            TileType[] types = { TileType.Finish, TileType.Floor, TileType.Hole, TileType.Wall };


            Assert.AreEqual(LevelBuilder.FloorTileset.Count, types.Length);

            for (int i = 0; i < LevelBuilder.FloorTileset.Count; i++)
            {
                Tile currentTile;
                LevelBuilder.FloorTileset.TryGetValue(FloorTileType.GetTypeOfTileNumber(i), out currentTile);
                Assert.AreEqual(types[i], currentTile.Type);
            }

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

            Assert.AreEqual(LevelBuilder.FloorTileset, level.Floor.Tileset);
            Assert.AreEqual(ftLevel0, level.Floor.FloorTypes);
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
        public void PassingTest()
        {
            Assert.Pass();
        }
    }
}
