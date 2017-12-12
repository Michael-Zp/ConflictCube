using ConflictCube.ComponentBased.Components;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.Geometry;

namespace ConflictCube.ComponentBased
{
    public class SceneBuilder
    {
        public static GameObject BuildScene(string level, Transform sceneTransform)
        {
            GameObject scene = new GameObject("Scene", sceneTransform, null, GameObjectType.Scene);

            Dictionary<GameObjectType, Material> floorMaterials = new Dictionary<GameObjectType, Material>();
            floorMaterials.Add(GameObjectType.Finish, new Material(Tilesets.FloorSheet.Tex, new Box2D(Tilesets.FloorSheet.CalcSpriteTexCoords(0)), Color.Blue));
            floorMaterials.Add(GameObjectType.Floor,  new Material(Tilesets.FloorSheet.Tex, new Box2D(Tilesets.FloorSheet.CalcSpriteTexCoords(1)), Color.Yellow));
            floorMaterials.Add(GameObjectType.Hole,   new Material(Tilesets.FloorSheet.Tex, new Box2D(Tilesets.FloorSheet.CalcSpriteTexCoords(2)), Color.Red));
            floorMaterials.Add(GameObjectType.Wall,   new Material(Tilesets.FloorSheet.Tex, new Box2D(Tilesets.FloorSheet.CalcSpriteTexCoords(3)), Color.Orange));

            scene.AddChild(FloorLoader.Instance(level, "Floor", new Transform(0, 0, 1, 1), scene, floorMaterials));
            scene.AddChild(Boundaries());

            return scene;
        }

        private static GameObject Boundaries()
        {
            GameObject Boundaries = new GameObject("Boundaries", new Transform(0, 0, 1, 1), null);
            Boundaries.AddChild(new Boundary("TopBoundary",    new Transform(0, 1f, 1f, 1f),   Boundaries, null, CollisionType.TopBoundary));
            Boundaries.AddChild(new Boundary("BottomBoundary", new Transform(0, -1f, 1f, 1f),  Boundaries, null, CollisionType.BottomBoundary));
            Boundaries.AddChild(new Boundary("RightBoundary",  new Transform(1f, 0f, 1f, 1f),  Boundaries, null, CollisionType.RightBoundary));
            Boundaries.AddChild(new Boundary("LeftBoundary",   new Transform(-1f, 0f, 1f, 1f), Boundaries, null, CollisionType.LeftBoundary));
            return Boundaries;
        }

    }
}