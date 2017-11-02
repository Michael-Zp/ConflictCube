using System;
using OpenTK;
using ConflictCube;
using NUnit.Framework;

namespace ConflictCubeTest
{
    using TileType = LevelBuilder.Tile.TileType;
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

            Assert.AreEqual(4, LevelBuilder.Tileset.Count);


            TileType[] types = { TileType.Finish, TileType.Floor, TileType.Hole, TileType.Wall };


            Assert.AreEqual(LevelBuilder.Tileset.Count, types.Length);

            for (int i = 0; i < LevelBuilder.Tileset.Count; i++)
            {
                LevelBuilder.Tile currentTile;
                LevelBuilder.Tileset.TryGetValue(i, out currentTile);
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


            Assert.AreEqual(LevelBuilder.Tileset, level.Tileset);
            Assert.AreEqual(level0, level.LevelTiles);
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
