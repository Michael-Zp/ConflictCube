using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.Objects;
using ConflictCube.ComponentBased.Model.Components.UI;
using ConflictCube.ComponentBased.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using ConflictCube.ResxFiles;
using ConflictCube.ComponentBased.Model.Components.Sound;

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
        public static Scene BuildMenu(IBuildScene buildScene, int windowWidth, int windowHeight, GameObject parent)
        {
            GameObject menu = new GameObject("UI", new Transform(), parent);

            new ColoredBox("Background", new Transform(), new Material(Color.FromArgb(255, 175, 175, 175)), menu);

            Camera camera = new Camera(new Transform(), menu, windowWidth, windowHeight, new Transform(), true);


            GameObject mainMenu = new GameObject("MainMenu", new Transform(), menu);
            GameObject controls = new GameObject("LevelSelectMenu", new Transform(), menu);
            GameObject levelSelect = new GameObject("LevelSelectMenu", new Transform(), menu);

            //Main menu

            ButtonGroup buttonsInMainMenu = new ButtonGroup("ButtonGroup", new Transform(), mainMenu);

            Action levelSelectOnClick = new Action(() => { mainMenu.Enabled = false; levelSelect.Enabled = true; });
            Action controlsOnClick = new Action(() => { mainMenu.Enabled = false; controls.Enabled = true; });
            Action exitOnClick = new Action(() => { Environment.Exit(0); });

            buttonsInMainMenu.AddButton(new Transform(0, .5f, .8f, .2f), new TextField("Level select", new Transform(0, .5f, 1.2f, .15f), "Level select", Font.Instance().NormalFont, null), levelSelectOnClick);
            buttonsInMainMenu.AddButton(new Transform(0, 0f, .8f, .2f), new TextField("Controls", new Transform(0, 0f, .7f, .15f), "Controls", Font.Instance().NormalFont, null), controlsOnClick);
            buttonsInMainMenu.AddButton(new Transform(0, -.5f, .8f, .2f), new TextField("Exit", new Transform(0, -.5f, .4f, .15f), "Exit", Font.Instance().NormalFont, null), exitOnClick);
            


            //Level select

            ButtonGroup buttonsInLevelSelect = new ButtonGroup("ButtonGroup", new Transform(), levelSelect);

            Action level1OnClick = new Action(() => { buildScene.BuildScene(LevelsWithNewTileset.FireIceFirstTestNewTileset); });
            Action level2OnClick = new Action(() => { buildScene.BuildScene(LevelsWithNewTileset.FireIceSecondTestNewTileset); });
            Action level3OnClick = new Action(() => { buildScene.BuildScene(LevelsWithNewTileset.YShiftTest); });
            Action level4OnClick = new Action(() => { buildScene.BuildScene(LevelsWithNewTileset.XShiftTest); });
            Action level5OnClick = new Action(() => { buildScene.BuildScene(LevelsWithNewTileset.level1); });

            buttonsInLevelSelect.AddButton(new Transform(0, .8f, .8f, .15f), new TextField("Level1", new Transform(0, .8f, .7f, .15f), "Level 1", Font.Instance().NormalFont, null), level1OnClick);
            buttonsInLevelSelect.AddButton(new Transform(0, .4f, .8f, .15f), new TextField("Level2", new Transform(0, .4f, .7f, .15f), "Level 2", Font.Instance().NormalFont, null), level2OnClick);
            buttonsInLevelSelect.AddButton(new Transform(0, .0f, .8f, .15f), new TextField("Level3", new Transform(0, 0f, .7f, .15f), "Level 3", Font.Instance().NormalFont, null), level3OnClick);
            buttonsInLevelSelect.AddButton(new Transform(0, -.4f, .8f, .15f), new TextField("Level4", new Transform(0, -.4f, .7f, .15f), "Level 4", Font.Instance().NormalFont, null), level4OnClick);
            buttonsInLevelSelect.AddButton(new Transform(0, -.8f, .8f, .15f), new TextField("Level5", new Transform(0, -.8f, .7f, .15f), "Level 5", Font.Instance().NormalFont, null), level5OnClick);
            

            Action gotoMainMenuOnEscape = new Action(() => { levelSelect.Enabled = false; controls.Enabled = false; mainMenu.Enabled = true; });

            DoActionOnButtonClick gotoMainMenuAction = new DoActionOnButtonClick("GotoMainMenu", new Transform(), OpenTK.Input.Key.BackSpace, gotoMainMenuOnEscape, KeyPressType.Release, levelSelect);
            
            levelSelect.Enabled = false;


            //Controls

            new TextField("Movement", new Transform(0, .7f, 2f, .2f), "Move with left thumb stick", Font.Instance().NormalFont, controls);
            new TextField("Porting0", new Transform(0, .3f, 2f, .2f), "Use Square, Triangle, and", Font.Instance().NormalFont, controls);
            new TextField("Porting1", new Transform(0, .1f, 2f, .2f), "Circle to change position", Font.Instance().NormalFont, controls);
            new TextField("Porting2", new Transform(0, -.1f, 1.8f, .2f), "with the other player.", Font.Instance().NormalFont, controls);
            new TextField("Smashing", new Transform(0, -.5f, 2f, .2f), "Use Cross to smash blocks.", Font.Instance().NormalFont, controls);
            new TextField("Smashing", new Transform(0, -.8f, 2.5f, .2f), "Backspace to go back in the menu.", Font.Instance().NormalFont, controls);

            DoActionOnButtonClick gotoMainMenuAction1 = new DoActionOnButtonClick("GotoMainMenu", new Transform(), OpenTK.Input.Key.BackSpace, gotoMainMenuOnEscape, KeyPressType.Release, controls);
                    
            controls.Enabled = false;
            
            return new Scene(menu, new List<Camera> { camera });
        }


        public static Scene BuildLevel(IBuildMenu buildMenu, string level, Transform sceneTransform, int windowWidth, int windowHeight, GameObject parent)
        {
            GameObject scene = new GameObject("Scene", sceneTransform, parent, GameObjectType.Scene);
            GameObject game = new GameObject("Game", new Transform(), scene);
            GameObject ui = new GameObject("UI", new Transform(), scene);
            
            CollisionGroup group = new CollisionGroup();
            Floor floor = FloorLoader.Instance(level, "Floor", new Transform(0, 0, 1, 1), group, game);
            Boundaries(group, floor, game);

            Camera mainCamera = InitializeMainCamera(windowWidth, windowHeight);
            mainCamera.RootGameObject = game;
            Camera uiCamera = InitializeUICamera(windowWidth, windowHeight);
            uiCamera.RootGameObject = ui;

            List<Player> players = InitializePlayers(floor);
            InitializeUI(ui);

            new GameManager("GameManager", players, buildMenu, ui);
            new IngameMenu("IngameMenu", ui, buildMenu);
            new PlayerSwitcher("PlayerSwitcher", players[0], players[1], game);

            CameraManager cameraManager = new CameraManager("CameraManager", new Transform(), new List<Camera>() { mainCamera }, players, scene);

            AudioPlayer bgMusic = new AudioPlayer(AudioResources.Background, true);
            bgMusic.PlayAudio();
            scene.AddComponent(bgMusic);


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


        private static GameObject InitializeUI(GameObject parent)
        {
            return new GameObject("UI", new Transform(), parent);
        }

        private static List<Player> InitializePlayers(Floor floor)
        {
            List<Player> players = new List<Player>();

            if (floor == null)
            {
                throw new Exception("Found no active floor for player 1.");
            }

            //Players
            players.Add(new OrangePlayer("FirePlayer", floor, floor));

            players.Add(new BluePlayer("IcePlayer", floor, floor));

            //Other Players
            players[0].OtherPlayer = players[1];
            players[1].OtherPlayer = players[0];

            return players;
        }
        
        private static GameObject Boundaries(CollisionGroup group, Floor floor, GameObject parent)
        {
            float minXFloor = floor.FloorTiles[0, 0].Transform.GetMinX(WorldRelation.Local);
            float minYFloor = floor.FloorTiles[floor.FloorRows - 1, 0].Transform.GetMinY(WorldRelation.Local);

            float middelOfFloor = floor.FloorRows * floor.FloorTileSize.Y;

            float maxXFloor = floor.FloorTiles[0, floor.FloorColumns - 1].Transform.GetMaxX(WorldRelation.Local);
            float maxYFloor = floor.FloorTiles[0, 0].Transform.GetMaxY(WorldRelation.Local);

            float widthOfLeftAndRightBoundary = .5f;
            float heightOfTopAndBottomBoundary = .25f;


            GameObject Boundaries = new GameObject("Boundaries", new Transform(0, 0, 1, 1), parent);

            GameObject topBoundary = new Boundary("TopBoundary", new Transform(minXFloor + (maxXFloor - minXFloor) / 2f, maxYFloor + heightOfTopAndBottomBoundary, (maxXFloor - minXFloor) / 2f, heightOfTopAndBottomBoundary), group, CollisionType.TopBoundary, Boundaries);
            GameObject bottomBoundary = new Boundary("BottomBoundary", new Transform(minXFloor + (maxXFloor - minXFloor) / 2f, minYFloor - heightOfTopAndBottomBoundary, (maxXFloor - minXFloor) / 2f, heightOfTopAndBottomBoundary), group, CollisionType.BottomBoundary, Boundaries);
            
            GameObject rightBoundary = new Boundary("RightBoundary", new Transform(maxXFloor + widthOfLeftAndRightBoundary, middelOfFloor, widthOfLeftAndRightBoundary, middelOfFloor), group, CollisionType.RightBoundary, Boundaries);
            GameObject leftBoundary = new Boundary("LeftBoundary", new Transform(minXFloor - widthOfLeftAndRightBoundary, middelOfFloor, widthOfLeftAndRightBoundary, middelOfFloor), group, CollisionType.LeftBoundary, Boundaries);

            if (DebugGame.ShowBoundaries)
            {
                topBoundary.AddComponent(new Material(Color.Red, null, null));
                bottomBoundary.AddComponent(new Material(Color.Green, null, null));
                rightBoundary.AddComponent(new Material(Color.Blue, null, null));
                leftBoundary.AddComponent(new Material(Color.Violet, null, null));
            }

            return Boundaries;
        }

    }
}