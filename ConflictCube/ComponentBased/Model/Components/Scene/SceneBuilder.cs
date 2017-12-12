using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Components.Objects.Tiles;
using System.Drawing;
using Zenseless.Geometry;

namespace ConflictCube.ComponentBased
{
    public class SceneBuilder
    {
        public static GameObject BuildScene(string level, Transform sceneTransform)
        {
            GameObject scene = new GameObject("Scene", sceneTransform, null, GameObjectType.Scene);
            
            FloorTile.FloorTileMaterials.Add(GameObjectType.Finish, new Material(Tilesets.Instance().FloorSheet.Tex, new Box2D(Tilesets.Instance().FloorSheet.CalcSpriteTexCoords(0)), Color.White));
            FloorTile.FloorTileMaterials.Add(GameObjectType.Floor,  new Material(Tilesets.Instance().FloorSheet.Tex, new Box2D(Tilesets.Instance().FloorSheet.CalcSpriteTexCoords(1)), Color.White));
            FloorTile.FloorTileMaterials.Add(GameObjectType.Hole,   new Material(Tilesets.Instance().FloorSheet.Tex, new Box2D(Tilesets.Instance().FloorSheet.CalcSpriteTexCoords(2)), Color.White));
            FloorTile.FloorTileMaterials.Add(GameObjectType.Wall,   new Material(Tilesets.Instance().FloorSheet.Tex, new Box2D(Tilesets.Instance().FloorSheet.CalcSpriteTexCoords(3)), Color.White));

            scene.AddChild(FloorLoader.Instance(level, "Floor", new Transform(0, 0, 1, 1), scene));
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