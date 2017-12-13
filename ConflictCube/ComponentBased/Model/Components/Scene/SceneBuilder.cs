using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    public class SceneBuilder
    {
        public static GameObject BuildScene(string level, Transform sceneTransform)
        {
            GameObject scene = new GameObject("Scene", sceneTransform, null, GameObjectType.Scene);
            CollisionGroup group = new CollisionGroup();

            scene.AddChild(FloorLoader.Instance(level, "Floor", new Transform(0, 0, 1, 1), scene, group));
            scene.AddChild(Boundaries(group));

            return scene;
        }

        private static GameObject Boundaries(CollisionGroup group)
        {
            GameObject Boundaries = new GameObject("Boundaries", new Transform(0, 0, 1, 1), null);
            Boundaries.AddChild(new Boundary("TopBoundary",    new Transform(0, 1f, 1f, 1f),   Boundaries, group, CollisionType.TopBoundary));
            Boundaries.AddChild(new Boundary("BottomBoundary", new Transform(0, -1f, 1f, 1f),  Boundaries, group, CollisionType.BottomBoundary));
            Boundaries.AddChild(new Boundary("RightBoundary",  new Transform(1f, 0f, 1f, 1f),  Boundaries, group, CollisionType.RightBoundary));
            Boundaries.AddChild(new Boundary("LeftBoundary",   new Transform(-1f, 0f, 1f, 1f), Boundaries, group, CollisionType.LeftBoundary));
            return Boundaries;
        }

    }
}