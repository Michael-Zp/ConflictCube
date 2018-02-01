using OpenTK;
using OpenTK.Input;
using System;
using System.Threading;

namespace ZenselessTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector2[] uvCoords = new Vector2[]
            {
                Vector2.Zero,
                Vector2.UnitX,
                Vector2.One,
                Vector2.UnitY
            };

            Vector2[] positions = new Vector2[]
            {
                new Vector2(-.5f),
                new Vector2(.5f, -.5f),
                new Vector2(.5f),
                new Vector2(-.5f, .5f)
            };

            Matrix4 camera = new Matrix4(1, 0, 0, 0f,
                                         0, 1, 0, 0,
                                         0, 0, -1, 0,
                                         0, 0, 0, 1);

            Vector4[] compound = new Vector4[]
            {
                new Vector4(positions[0].X, positions[0].Y, 0, 1),
                new Vector4(positions[1].X, positions[1].Y, 0, 1),
                new Vector4(positions[2].X, positions[2].Y, 0, 1),
                new Vector4(positions[3].X, positions[3].Y, 0, 1),
            };

            for(int i = 0; i < 4; i++)
            {
                float x = camera[0, 0] * compound[i].X + camera[1, 0] * compound[i].Y + camera[2, 0] * compound[i].Z + camera[3, 0] * compound[i].W;
                float y = camera[0, 1] * compound[i].X + camera[1, 1] * compound[i].Y + camera[2, 1] * compound[i].Z + camera[3, 1] * compound[i].W;
                float z = camera[0, 2] * compound[i].X + camera[1, 2] * compound[i].Y + camera[2, 2] * compound[i].Z + camera[3, 2] * compound[i].W;
                float w = camera[0, 3] * compound[i].X + camera[1, 3] * compound[i].Y + camera[2, 3] * compound[i].Z + camera[3, 3] * compound[i].W;

                Vector4 tV = new Vector4(x, y, z, w);
                

                Console.WriteLine(tV);
            }

            while(true)
            {

            }

        }
    }
}

