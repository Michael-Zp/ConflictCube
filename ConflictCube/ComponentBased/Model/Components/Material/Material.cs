using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.Geometry;
using Zenseless.HLGL;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased.Components
{
    public class Material : Component
    {
        public int ID;

        public ITexture Texture {
            get {
                ITexture tex;
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
                    Console.WriteLine("Texture - Did not find material number " + ID);
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
                    Console.WriteLine("UVCoordinates - Did not find material number " + ID);
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
                    Console.WriteLine("Color - Did not find material number " + ID);
                }
                return Color.FromArgb(1, 1, 1, 1);
            }
        }

        public IShader Shader {
            get {
                IShader shader;
                try
                {
                    shader = Materials.GetMaterialData(ID).Shader;
                    return shader;
                }
                catch (Exception)
                {
                    Console.WriteLine("ShaderText - Did not find material number " + ID);
                }
                return null;
            }
        }

        public List<Tuple<string, float>> ShaderParameters1D = new List<Tuple<string, float>>();
        public List<Tuple<string, Vector2>> ShaderParameters2D = new List<Tuple<string, Vector2>>();
        public List<Tuple<string, Vector3>> ShaderParameters3D = new List<Tuple<string, Vector3>>();
        public List<Tuple<string, Vector4>> ShaderParameters4D = new List<Tuple<string, Vector4>>();


        public void AddShaderParameter(string parameterName, float value)
        {
            foreach (Tuple<string, float> param in ShaderParameters1D)
            {
                if (param.Item1 == parameterName)
                {
                    ShaderParameters1D.Remove(param);
                    break;
                }
            }

            ShaderParameters1D.Add(Tuple.Create(parameterName, value));
        }

        public void AddShaderParameter(string parameterName, Vector2 value)
        {
            foreach (Tuple<string, Vector2> param in ShaderParameters2D)
            {
                if (param.Item1 == parameterName)
                {
                    ShaderParameters2D.Remove(param);
                    break;
                }
            }

            ShaderParameters2D.Add(Tuple.Create(parameterName, value));
        }

        public void AddShaderParameter(string parameterName, Vector3 value)
        {
            foreach (Tuple<string, Vector3> param in ShaderParameters3D)
            {
                if (param.Item1 == parameterName)
                {
                    ShaderParameters3D.Remove(param);
                    break;
                }
            }

            ShaderParameters3D.Add(Tuple.Create(parameterName, value));
        }

        public void AddShaderParameter(string parameterName, Vector4 value)
        {
            foreach (Tuple<string, Vector4> param in ShaderParameters4D)
            {
                if (param.Item1 == parameterName)
                {
                    ShaderParameters4D.Remove(param);
                    break;
                }
            }

            ShaderParameters4D.Add(Tuple.Create(parameterName, value));
        }


        public Material(Color color) : this(color, null, null, null)
        {}

        public Material(Color color, string shaderContents) : this(color, null, null, shaderContents)
        {}

        public Material(Color color, ITexture texture, Box2D uvCoordinates) : this(color, texture, uvCoordinates, null)
        {}

        public Material(Color color, ITexture texture, Box2D uvCoordinates, string fragmentShader)
        {
            IShader shader;

            if(String.IsNullOrEmpty(fragmentShader))
            {
                shader = null;
            }
            else
            {
                shader = ShaderLoader.FromStrings(DefaultShader.VertexShaderScreenQuad, fragmentShader);
            }


            ID = Materials.AddMaterialData(new MaterialData(texture, uvCoordinates, color, shader));
        }

        public override Component Clone()
        {
            Material newMaterial = (Material)base.Clone();

            newMaterial.ID = ID;

            return newMaterial;
        }
    }
}
