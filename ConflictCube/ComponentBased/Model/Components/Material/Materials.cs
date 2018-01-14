using System;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased.Components
{
    

    public static class Materials
    {
        private static Dictionary<int, MaterialData> AllMaterials = new Dictionary<int, MaterialData>();
        private static int CurrentID = 0;


        /// <summary>
        ///     Generates a new MaterialData in the AllMaterials list.
        ///     Returns the ID of the Material.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddMaterialData(MaterialData data)
        {
            int ID = CurrentID;
            AllMaterials.Add(ID, data);
            CurrentID++;

            return ID;
        }

        public static MaterialData GetMaterialData(int ID)
        {
            MaterialData matDat;
            try
            {
                AllMaterials.TryGetValue(ID, out matDat);
                return matDat;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static void SetMaterialColor(int ID, Color color)
        {
            MaterialData matData = GetMaterialData(ID);
            AllMaterials.Remove(ID);
            matData.Color = color;
            AllMaterials.Add(ID, matData);
        }
    }
}
