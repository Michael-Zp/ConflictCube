using ConflictCube.ResxFiles;
using Engine.Components;
using Engine.Time;
using System;
using System.Drawing;
using Zenseless.OpenGL;

namespace ConflictCube.Objects
{
    public class UseField : GameObject
    {
        private static bool MaterialsAreInitialized = false;
        private static Material ForegroundMaterial;
        private static Material BackgroundMaterial;

        private static int MaxAlpha = 150;

        public UseField(string name, Transform transform, GameObject parent) : base(name, transform, parent)
        {
            if (!MaterialsAreInitialized)
            {
                MaterialsAreInitialized = true;
                ForegroundMaterial = new Material(Color.FromArgb(255, Color.White), TextureLoader.FromBitmap(TexturResource.UseFieldIndicator), new Zenseless.Geometry.Box2D(0, 0, 1, 1));
                BackgroundMaterial = new Material(Color.FromArgb(MaxAlpha, 0, 255, 64));
            }

            AddComponent(BackgroundMaterial);
            AddComponent(ForegroundMaterial);
        }

        public override void OnUpdate()
        {
            float alpha = 0.1f * (float)Math.Cos(3.5f * Time.CurrentTime) + 0.9f;
            int iAlpha = (int)(Math.Floor(alpha * 255));
            
            ForegroundMaterial.Color = Color.FromArgb(iAlpha, ForegroundMaterial.Color);
            
            iAlpha = (int)(Math.Floor(alpha * MaxAlpha));

            BackgroundMaterial.Color = Color.FromArgb(iAlpha, BackgroundMaterial.Color);

        }
    }
}
