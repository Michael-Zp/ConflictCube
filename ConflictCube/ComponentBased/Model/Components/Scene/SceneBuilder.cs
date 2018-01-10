using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased
{
    public class SceneBuilder
    {
        public static GameObject BuildScene(string level, Transform sceneTransform)
        {
            GameObject scene = new GameObject("Scene", sceneTransform, null, GameObjectType.Scene);
            CollisionGroup group = new CollisionGroup();

            Floor floor = FloorLoader.Instance(level, "Floor", new Transform(0, 0, 1, 1), scene, group);
            scene.AddChild(floor);
            scene.AddChild(Boundaries(group, floor));

            return scene;
        }


        //TODO: Transforms are not right... Something went terribly wrong in the ToGlobal and ToLocal... Has to be fixxed !!!!
        private static GameObject Boundaries(CollisionGroup group, Floor floor)
        {
            float minXFloor = floor.FloorTiles[0, 0].Transform.GetMinX(WorldRelation.Local);
            float minYFloor = floor.FloorTiles[floor.FloorRows - 1, 0].Transform.GetMinY(WorldRelation.Local);

            float middelOfFloor = floor.FloorRows * floor.FloorTileSize.Y;

            float maxXFloor = floor.FloorTiles[0, floor.FloorColumns - 1].Transform.GetMaxX(WorldRelation.Local);
            float maxYFloor = floor.FloorTiles[0, 0].Transform.GetMaxY(WorldRelation.Local);

            float widthOfLeftAndRightBoundary = .5f;
            float heightOfTopAndBottomBoundary = .25f;


            GameObject Boundaries = new GameObject("Boundaries", new Transform(0, 0, 1, 1), null);

            GameObject topBoundary = new Boundary("TopBoundary",        new Transform( minXFloor + (maxXFloor - minXFloor) / 2f, maxYFloor + heightOfTopAndBottomBoundary, (maxXFloor - minXFloor) / 2f, heightOfTopAndBottomBoundary), Boundaries, group, CollisionType.TopBoundary);
            GameObject bottomBoundary = new Boundary("BottomBoundary",  new Transform( minXFloor + (maxXFloor - minXFloor) / 2f, minYFloor - heightOfTopAndBottomBoundary, (maxXFloor - minXFloor) / 2f, heightOfTopAndBottomBoundary), Boundaries, group, CollisionType.BottomBoundary);


            GameObject rightBoundary = new Boundary("RightBoundary",    new Transform( maxXFloor + widthOfLeftAndRightBoundary, middelOfFloor, widthOfLeftAndRightBoundary, middelOfFloor), Boundaries, group, CollisionType.RightBoundary);
            GameObject leftBoundary = new Boundary("LeftBoundary",      new Transform( minXFloor - widthOfLeftAndRightBoundary, middelOfFloor, widthOfLeftAndRightBoundary, middelOfFloor), Boundaries, group, CollisionType.LeftBoundary);

            if(DebugGame.ShowBoundaries)
            {
                topBoundary.AddComponent(new Material(System.Drawing.Color.Red, null, null));
                bottomBoundary.AddComponent(new Material(System.Drawing.Color.Green, null, null));
                rightBoundary.AddComponent(new Material(System.Drawing.Color.Blue, null, null));
                leftBoundary.AddComponent(new Material(System.Drawing.Color.Violet, null, null));
            }

            Boundaries.AddChild(topBoundary);
            Boundaries.AddChild(bottomBoundary);
            Boundaries.AddChild(rightBoundary);
            Boundaries.AddChild(leftBoundary);

            return Boundaries;
        }

    }
}