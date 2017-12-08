using ConflictCube.ComponentBased.Components;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.Geometry;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased
{
    public class SceneBuilder
    {
        private static string LevelDirectoryPath = ".\\ConflictCube\\Levels\\";


        public static GameObject BuildScene(string level)
        {
            GameObject scene = new GameObject("Scene", new Transform(0, 0, 2, 2), null, GameObjectType.Scene);

            Dictionary<GameObjectType, Material> floorMaterials = new Dictionary<GameObjectType, Material>();
            floorMaterials.Add(GameObjectType.Finish, new Material((Texture)Tilesets.FloorSheet.Tex, new Box2D(Tilesets.FloorSheet.CalcSpriteTexCoords(0)), Color.White));
            floorMaterials.Add(GameObjectType.Floor,  new Material((Texture)Tilesets.FloorSheet.Tex, new Box2D(Tilesets.FloorSheet.CalcSpriteTexCoords(1)), Color.White));
            floorMaterials.Add(GameObjectType.Hole,   new Material((Texture)Tilesets.FloorSheet.Tex, new Box2D(Tilesets.FloorSheet.CalcSpriteTexCoords(2)), Color.White));
            floorMaterials.Add(GameObjectType.Wall,   new Material((Texture)Tilesets.FloorSheet.Tex, new Box2D(Tilesets.FloorSheet.CalcSpriteTexCoords(3)), Color.White));

            scene.AddChild(FloorLoader.Instance(level, "Floor", new Transform(0, 0, 1.6f, 1.6f), scene, floorMaterials));
            
            return scene;
        }

    }
}