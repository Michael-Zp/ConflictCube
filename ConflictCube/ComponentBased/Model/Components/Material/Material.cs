using System;
using System.Drawing;
using Zenseless.Geometry;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased.Components
{
    public class Material : Component
    {
        public int ID;

        public Texture Texture {
            get {
                Texture tex;
                try
                {
                    tex = Materials.GetMaterialData(ID)?.Texture;
                    if(tex != null)
                    {
                        return tex;
                    }
                    return null;
                }
                catch(Exception)
                {
                    Console.WriteLine("Did not find material number " + ID);
                }
                return null;
            }
        }

        public Box2D UVCoordinates {
            get {
                Box2D uvCoordinates;
                try
                {
                    uvCoordinates = Materials.GetMaterialData(ID)?.UVCoordinates;
                    if (uvCoordinates != null)
                    {
                        return uvCoordinates;
                    }
                    return null;
                }
                catch (Exception)
                {
                    Console.WriteLine("Did not find material number " + ID);
                }
                return null;
            }
        }

        public Color Color {
            get {
                Color color;
                try
                {
                    color = Materials.GetMaterialData(ID).Color;
                    return color;
                }
                catch (Exception)
                {
                    Console.WriteLine("Did not find material number " + ID);
                }
                return Color.FromArgb(1, 1, 1, 1);
            }
        }

        public Material(Texture texture, Box2D uvCoordinates, Color color)
        {
            ID = Materials.AddMaterialData(new MaterialData(texture, uvCoordinates, color));
        }

        public override Component Clone()
        {
            Material newMaterial = (Material)base.Clone();

            newMaterial.ID = ID;

            return newMaterial;
        }
    }
}
