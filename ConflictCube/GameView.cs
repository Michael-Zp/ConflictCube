using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Zenseless.OpenGL;
using Zenseless.HLGL;
using Zenseless.Geometry;
using System.Drawing;

namespace ConflictCube
{
    public class GameView
    {
        private MyWindow Window;

        public GameView(MyWindow window)
        {
            Window = window;

            Window.Resize += (s, a) => GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.ClearColor(Color4.CornflowerBlue);
        }

        public void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        
        public void ShowLevel(Level currentLevel)
        {
            int rows = currentLevel.LevelTiles.GetLength(0);
            int columns = currentLevel.LevelTiles.GetLength(1);
            LevelBuilder.Tile currentTile;
            Vector2 tileSizeInScreen = new Vector2(2 / (float)columns, 2 / (float)rows);
            
            for (int y = 0; y < rows; y++)
            {
                float posY = 1 - (y + 1) * tileSizeInScreen.Y;
                for (int x = 0; x < columns; x++)
                {
                    currentLevel.Tileset.TryGetValue(currentLevel.LevelTiles[y, x], out currentTile);

                    float posX = -1 + x * tileSizeInScreen.X;

                    OpenTKWrapper.DrawBoxWithTexture(new Box2D(posX, posY, tileSizeInScreen.X, tileSizeInScreen.Y), currentTile.Texture);
                }
            }
        }

        public void CloseWindow()
        {
            Window.Close();
        }
    }
}
