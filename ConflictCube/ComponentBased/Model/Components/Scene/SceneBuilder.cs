using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.Objects;
using ConflictCube.ComponentBased.Model.Components.UI;
using ConflictCube.ComponentBased.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.OpenGL;
using ConflictCube.ResxFiles;

namespace ConflictCube.ComponentBased
{
    public struct Scene
    {
        public GameObject RootGameObject;
        public List<Camera> Cameras;

        public Scene(GameObject rootGameObject, List<Camera> cameras)
        {
            RootGameObject = rootGameObject;
            Cameras = cameras;
        }
    }

    public class SceneBuilder
    {
        public static Scene BuildMenu(GameState state, int windowWidth, int windowHeight)
        {
            GameObject menu = new GameObject("UI", new Transform());

            menu.AddChild(new ColoredBox("Background", new Transform(), new Material(Color.FromArgb(255, 175, 175, 175))));

            Camera camera = new Camera(new Transform(), menu, windowWidth, windowHeight, new Transform(), true);


            GameObject mainMenu = new GameObject("MainMenu", new Transform());
            GameObject controls = new GameObject("LevelSelectMenu", new Transform());
            GameObject levelSelect = new GameObject("LevelSelectMenu", new Transform());

            //Main menu
            
            ButtonGroup buttonsInMainMenu = new ButtonGroup("ButtonGroup", new Transform());

            Action levelSelectOnClick = new Action(() => { mainMenu.Enabled = false; levelSelect.Enabled = true; });
            Action controlsOnClick = new Action(() => { mainMenu.Enabled = false; controls.Enabled = true; });
            Action exitOnClick = new Action(() => { Environment.Exit(0); });

            buttonsInMainMenu.AddButton(new Transform(0,  .5f, .8f, .2f), new TextField("Level select", new Transform(0, .5f, 1.2f, .15f), "Level select", Font.Instance().NormalFont), levelSelectOnClick);
            buttonsInMainMenu.AddButton(new Transform(0,   0f, .8f, .2f), new TextField("Controls", new Transform(0, 0f, .7f, .15f), "Controls", Font.Instance().NormalFont), controlsOnClick);
            buttonsInMainMenu.AddButton(new Transform(0, -.5f, .8f, .2f), new TextField("Exit", new Transform(0, -.5f, .4f, .15f), "Exit", Font.Instance().NormalFont), exitOnClick);

            mainMenu.AddChild(buttonsInMainMenu);

            menu.AddChild(mainMenu);


            //Level select
            
            ButtonGroup buttonsInLevelSelect = new ButtonGroup("ButtonGroup", new Transform());

            Action level1OnClick = new Action(() => { state.BuildScene(LevelsWithNewTileset.XShiftTest); });
            Action level2OnClick = new Action(() => { state.BuildScene(LevelsWithNewTileset.YShiftTest); });
            Action level3OnClick = new Action(() => { state.BuildScene(LevelsWithNewTileset.FireIceFirstTestNewTileset); });

            buttonsInLevelSelect.AddButton(new Transform(0,  .5f, .8f, .2f), new TextField("Level1", new Transform(0,  .5f,  .7f, .15f), "Level 1", Font.Instance().NormalFont), level1OnClick);
            buttonsInLevelSelect.AddButton(new Transform(0,   0f, .8f, .2f), new TextField("Level2", new Transform(0,   0f,  .7f, .15f), "Level 2", Font.Instance().NormalFont), level2OnClick);
            buttonsInLevelSelect.AddButton(new Transform(0, -.5f, .8f, .2f), new TextField("Level3", new Transform(0, -.5f,  .7f, .15f), "Level 3", Font.Instance().NormalFont), level3OnClick);

            levelSelect.AddChild(buttonsInLevelSelect);

            Action gotoMainMenuOnEscape = new Action(() => { levelSelect.Enabled = false; controls.Enabled = false; mainMenu.Enabled = true; });

            DoActionOnButtonClick gotoMainMenuAction = new DoActionOnButtonClick("GotoMainMenu", new Transform(), OpenTK.Input.Key.BackSpace, gotoMainMenuOnEscape, KeyPressType.Release);

            levelSelect.AddChild(gotoMainMenuAction);

            menu.AddChild(levelSelect);

            levelSelect.Enabled = false;


            //Controls

            controls.AddChild(new TextField("Movement", new Transform(0, .7f, 2f, .2f), "Move with left thumb stick", Font.Instance().NormalFont));
            controls.AddChild(new TextField("Porting0", new Transform(0, .3f, 2f, .2f), "Use Square, Triangle, and", Font.Instance().NormalFont));
            controls.AddChild(new TextField("Porting1", new Transform(0, .1f, 2f, .2f), "Circle to change position", Font.Instance().NormalFont));
            controls.AddChild(new TextField("Porting2", new Transform(0, -.1f, 1.8f, .2f), "with the other player.", Font.Instance().NormalFont));
            controls.AddChild(new TextField("Smashing", new Transform(0, -.5f, 2f, .2f), "Use Cross to smash blocks.", Font.Instance().NormalFont));
            controls.AddChild(new TextField("Smashing", new Transform(0, -.8f, 2.5f, .2f), "Backspace to go back in the menu.", Font.Instance().NormalFont));

            DoActionOnButtonClick gotoMainMenuAction1 = new DoActionOnButtonClick("GotoMainMenu", new Transform(), OpenTK.Input.Key.BackSpace, gotoMainMenuOnEscape, KeyPressType.Release);

            controls.AddChild(gotoMainMenuAction1);
            

            menu.AddChild(controls);

            controls.Enabled = false;


            return new Scene(menu, new List<Camera> { camera });
        }


        public static Scene BuildLevel(string level, Transform sceneTransform, int windowWidth, int windowHeight)
        {
            GameObject scene = new GameObject("Scene", sceneTransform, null, GameObjectType.Scene);
            GameObject game = new GameObject("Game", new Transform());
            GameObject ui = new GameObject("UI", new Transform());

            scene.AddChild(ui);
            scene.AddChild(game);

            CollisionGroup group = new CollisionGroup();
            Floor floor = FloorLoader.Instance(level, "Floor", new Transform(0, 0, 1, 1), scene, group);
            game.AddChild(floor);
            game.AddChild(Boundaries(group, floor));

            /*
            Floor floor = (Floor)Game.FindGameObjectByTypeInChildren<Floor>();
            Transform buttonTransform = floor.GetBoxAtGridPosition(new Vector2(1, 5));
            buttonTransform.SetSize(buttonTransform.GetSize(WorldRelation.Global) / 2, WorldRelation.Global);
            buttonTransform = floor.Transform.TransformToLocal(buttonTransform);

            OnButtonChangeFloorEvent changeFloorEvent = new OnButtonChangeFloorEvent(floor);
            changeFloorEvent.AddChangeOnFloor(1, 2, GameObjectType.OrangeFloor);
            changeFloorEvent.AddChangeOnFloor(2, 2, GameObjectType.OrangeFloor);
            changeFloorEvent.AddChangeOnFloor(3, 2, GameObjectType.OrangeFloor);
            changeFloorEvent.AddChangeOnFloor(4, 2, GameObjectType.OrangeFloor);
            changeFloorEvent.AddChangeOnFloor(5, 2, GameObjectType.OrangeFloor);
            changeFloorEvent.AddChangeOnFloor(6, 2, GameObjectType.OrangeFloor);

            Button = new Button("button", buttonTransform, changeFloorEvent, floor.CollisionGroup);
            floor.AddChild(Button);
            */

            Camera mainCamera = InitializeMainCamera(windowWidth, windowHeight);
            mainCamera.RootGameObject = game;
            Camera uiCamera = InitializeUICamera(windowWidth, windowHeight);
            uiCamera.RootGameObject = ui;
            List<Player> players = InitializePlayers(floor);
            ui.AddChild(InitializeUI());
            ui.AddChild(InitializeGameOverScreen(players));


            CameraManager cameraManager = new CameraManager("CameraManager", new Transform(), new List<Camera>() { mainCamera }, players);
            scene.AddChild(cameraManager);

            return new Scene(scene, new List<Camera> { mainCamera, uiCamera });
        }


        private static Camera InitializeUICamera(int windowWidth, int windowHeight)
        {
            return new Camera(new Transform(0, 0, 1f, 1f), null, windowWidth, windowHeight, new Transform(0, 0, 1f, 1f), true);
        }

        private static Camera InitializeMainCamera(int windowWidth, int windowHeight)
        {
            return new Camera(new Transform(0, 0, 1f, 1f), null, windowWidth, windowHeight, new Transform(0f, 0f, 1f, 1f), false);
        }


        private static GameObject InitializeUI()
        {
            return new GameObject("UI", new Transform());
        }

        private static List<Player> InitializePlayers(Floor floor)
        {
            List<Player> players = new List<Player>();
            Material playerMat = new Material(Color.White, (Texture)Tilesets.Instance().NewPlayerSheet.Tex, Tilesets.Instance().NewPlayerSheet.CalcSpriteTexCoords(0));
            Material playerOrangeMat = new Material(Color.FromArgb(128, Color.Orange), null, null);
            Material playerBlueMat = new Material(Color.FromArgb(128, Color.DarkBlue), null, null);
            Material playerGhostMat = new Material(Color.FromArgb(64, 255, 255, 255), (Texture)Tilesets.Instance().NewPlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0));
            
            if (floor == null)
            {
                throw new Exception("Found no active floor for player 1.");
            }

            //Players
            BoxCollider Player1Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, floor.CollisionGroup, CollisionType.PlayerFire);
            players.Add(new OrangePlayer("FirePlayer", new Transform(0, 0, .06f, .06f), Player1Collider, playerMat, floor, floor, 20f, GameObjectType.PlayerFire, null));
            floor.AddChild(players[0]);
            players[0].ResetToLastCheckpoint();
            players[0].AddChild(new ColoredBox("Player0Orange", new Transform(), playerOrangeMat));

            BoxCollider Player2Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, floor.CollisionGroup, CollisionType.PlayerIce);
            players.Add(new BluePlayer("IcePlayer", new Transform(0, 0, .06f, .06f), Player2Collider, playerMat, floor, floor, 20f, GameObjectType.PlayerIce, null));
            floor.AddChild(players[1]);
            players[1].ResetToLastCheckpoint();
            players[1].AddChild(new ColoredBox("Player1Blue", new Transform(), playerBlueMat));

            //Other Players
            players[0].OtherPlayer = players[1];
            players[1].OtherPlayer = players[0];

            return players;
        }


        private static GameObject InitializeGameOverScreen(List<Player> players)
        {
            GameOverScreen gos = new GameOverScreen("GameOverScreen", new Transform());
            gos.Enabled = false;

            foreach (Player player in players)
            {
                player.GameOverScreen = gos;
            }

            return gos;
        }


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