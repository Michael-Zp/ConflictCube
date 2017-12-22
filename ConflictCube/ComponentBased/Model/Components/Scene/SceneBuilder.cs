using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    public class SceneBuilder
    {
        public static GameObject BuildScene(string level, Transform sceneTransform, bool debugBoundaries = false)
        {
            GameObject scene = new GameObject("Scene", sceneTransform, null, GameObjectType.Scene);
            CollisionGroup group = new CollisionGroup();

            Floor floor = FloorLoader.Instance(level, "Floor", new Transform(0, 0, 1, 1), scene, group);
            scene.AddChild(floor);
            scene.AddChild(Boundaries(group, floor, debugBoundaries));

            return scene;
        }


        //TODO: Transforms are not right... Something went terribly wrong in the ToGlobal and ToLocal... Has to be fixxed !!!!
        private static GameObject Boundaries(CollisionGroup group, Floor floor, bool debugBoundaries)
        {
            int alpha = debugBoundaries ? 128 : 0;

            float middelOfFloor = floor.FloorRows * floor.FloorTileSize.Y;
            

            GameObject Boundaries = new GameObject("Boundaries", new Transform(0, 0, 1, 1), null);

            GameObject topBoundary = new Boundary("TopBoundary", new Transform(0, middelOfFloor * 2 + 1, 1, 1f), Boundaries, group, CollisionType.TopBoundary);
            topBoundary.AddComponent(new Material(null, null, System.Drawing.Color.FromArgb(alpha, System.Drawing.Color.Red)));

            GameObject bottomBoundary = new Boundary("BottomBoundary", new Transform(0, -1f, 1f, 1f), Boundaries, group, CollisionType.BottomBoundary);
            bottomBoundary.AddComponent(new Material(null, null, System.Drawing.Color.FromArgb(alpha, System.Drawing.Color.Green)));

            GameObject rightBoundary = new Boundary("RightBoundary", new Transform(2f, middelOfFloor, 1f, middelOfFloor), Boundaries, group, CollisionType.RightBoundary);
            rightBoundary.AddComponent(new Material(null, null, System.Drawing.Color.FromArgb(alpha, System.Drawing.Color.Blue)));

            GameObject leftBoundary = new Boundary("LeftBoundary", new Transform(-2f, middelOfFloor, 1f, middelOfFloor), Boundaries, group, CollisionType.LeftBoundary);
            leftBoundary.AddComponent(new Material(null, null, System.Drawing.Color.FromArgb(alpha, System.Drawing.Color.Violet)));

            Boundaries.AddChild(topBoundary);
            Boundaries.AddChild(bottomBoundary);
            Boundaries.AddChild(rightBoundary);
            Boundaries.AddChild(leftBoundary);

            return Boundaries;
        }

    }
}