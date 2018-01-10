using System.Collections.Generic;
using System;
using ConflictCube.ComponentBased.Components;
using Zenseless.OpenGL;
using System.Drawing;
using ConflictCube.ComponentBased.Controller;
using OpenTK;
using ConflictCube.ComponentBased.Model.Components.Objects;
using ConflictCube.ComponentBased.View;

namespace ConflictCube.ComponentBased
{
    public class GameState
    {
        public Camera UICamera;
        public Camera Player1Camera;
        public Camera Player2Camera;
        public Game Game { get; set; }
        public List<Player> Players { get; private set; }

        private Transform UiPlayer1Transform = new Transform(-.9f, 0f, .1f, 1f);
        private Transform ScenePlayer1Transform = new Transform(0f, 0, 1f, 1f);

        private Transform UiPlayer2Transform = new Transform(.9f, 0f, .1f, 1f);
        private Transform ScenePlayer2Transform = new Transform(0f, 0, 1f, 1f);



        private Button Button;

        public GameState(int windowWidth, int windowHeight)
        {
            SetUpCameras(windowWidth, windowHeight);

            Game = new Game("Game", new Transform(0, 0, 1, 1));

            GameObject scene = SceneBuilder.BuildScene(Levels.FireIceFourthTest, new Transform());
            Game.AddChild(scene);
            Player1Camera.RootGameObject = scene;
            Player2Camera.RootGameObject = scene;

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

            InitializePlayers();
            InitializeUI();

            
            GameObject testGo = new GameObject("Test", new Transform(0, 0, .2f, .07f));
            scene.AddChild(testGo);
            testGo.AddComponent(new Material(Color.White, ShaderResources.Afterglow));
            testGo.GetComponent<Material>().ShaderParameters1D.Add(Tuple.Create("startTime", Time.Time.CurrentTime));
            testGo.GetComponent<Material>().ShaderParameters1D.Add(Tuple.Create("direction", 1f));
            testGo.GetComponent<Material>().ShaderParameters1D.Add(Tuple.Create("lifetime", 1f));
            testGo.GetComponent<Material>().ShaderParameters3D.Add(Tuple.Create("desiredColor", new Vector3(Color.Orange.R, Color.Orange.G, Color.Orange.B)));


            /*
            GameObject test15Go = new GameObject("Test15", new Transform(-1f, 0, .25f, .5f));
            testGo.AddChild(test15Go);
            test15Go.AddComponent(new Material(Color.Orange));

            GameObject test2Go = new GameObject("Test2", new Transform(0, 0, .5f, .5f));
            testGo.AddChild(test2Go);
            test2Go.AddComponent(new Material(Color.White, Tilesets.Instance().PlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0)));

            GameObject test25Go = new GameObject("Test25", new Transform(1f, 0, .25f, .5f));
            testGo.AddChild(test25Go);
            test25Go.AddComponent(new Material(Color.Green));

            GameObject test3Go = new GameObject("Test3", new Transform(.5f, .5f, .5f, .5f));
            testGo.AddChild(test3Go);
            test3Go.AddComponent(new Material(Color.White, Tilesets.Instance().PlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0)));

            UICamera.RootGameObject = testGo;
            */
        }

        private void SetUpCameras(int windowWidth, int windowHeight)
        {
            UICamera = new Camera(new Transform(), null, windowWidth, windowHeight, new Transform(0, 0, 1f, 1f), true);

            Player1Camera = new Camera(new Transform(), null, windowWidth, windowHeight, new Transform(-.42f, 0f, .5f, 1f), false);
            Player2Camera = new Camera(new Transform(), null, windowWidth, windowHeight, new Transform( .42f, 0f, .5f, 1f), false);
        }
        

        private void InitializeUI()
        {
            GameObject ui = new GameObject("UI", new Transform());

            GameObject player1UI = new PlayerUI("Player0UI", Players[0], UiPlayer1Transform, ui);
            ui.AddChild(player1UI);
            
            GameObject player2UI = new PlayerUI("Player0UI", Players[1], UiPlayer2Transform, ui);
            ui.AddChild(player2UI);

            UICamera.RootGameObject = ui;
        }

        public void InitializePlayers()
        {
            Players = new List<Player>();
            Material playerMat = new Material(Color.White, (Texture)Tilesets.Instance().PlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0));
            Material playerOrangeMat = new Material(Color.FromArgb(128, Color.Orange), null, null);
            Material playerBlueMat = new Material(Color.FromArgb(128, Color.DarkBlue), null, null);
            Material playerGhostMat = new Material(Color.FromArgb(64, 255, 255, 255), (Texture)Tilesets.Instance().PlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0));

            Floor floor = (Floor)Game.FindGameObjectByTypeInChildren<Floor>();

            if (floor == null)
            {
                throw new Exception("Found no active floor for player 1.");
            }
            


            //Players
            BoxCollider Player1Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, floor.CollisionGroup, CollisionType.PlayerFire);
            Players.Add(new OrangePlayer("FirePlayer", new Transform(0, 0, .06f, .06f), Player1Collider, playerMat, floor, floor, .2f, GameObjectType.PlayerFire, null));
            floor.AddChild(Players[0]);
            Players[0].ResetToLastCheckpoint();
            Players[0].AddChild(new ColoredBox("Player0Orange", new Transform(), playerOrangeMat, Players[0]));

            BoxCollider Player2Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, floor.CollisionGroup, CollisionType.PlayerIce);
            Players.Add(new BluePlayer("IcePlayer", new Transform(0, 0, .06f, .06f), Player2Collider, playerMat, floor, floor, .2f, GameObjectType.PlayerIce, null));
            floor.AddChild(Players[1]);
            Players[1].ResetToLastCheckpoint();
            Players[1].AddChild(new ColoredBox("Player1Blue", new Transform(), playerBlueMat, Players[1]));

            //Other Players
            Players[0].OtherPlayer = Players[1];
            Players[1].OtherPlayer = Players[0];

            //Ghost Players
            floor.AddChild(new GhostPlayer("GhostPlayer2OnArea1", new Transform(0, 0, .06f, .06f), floor, playerGhostMat, Players[1], GameObjectType.GhostPlayer));
            floor.AddChild(new GhostPlayer("GhostPlayer1OnArea2", new Transform(0, 0, .06f, .06f), floor, playerGhostMat, Players[0], GameObjectType.GhostPlayer));
        }


        public void UpdateAll()
        {
            Game.UpdateAll();

            CheckLooseCondition();

            Vector2 player1Pos = Players[0].Transform.GetPosition(WorldRelation.Global);
            Vector2 player2Pos = Players[1].Transform.GetPosition(WorldRelation.Global);

            Player1Camera.Transform.SetPosition(new Vector2(-player1Pos.X, -player1Pos.Y), WorldRelation.Global);
            Player2Camera.Transform.SetPosition(new Vector2(-player2Pos.X, -player2Pos.Y), WorldRelation.Global);

            //GameView.DrawDebug(Button.Transform.TransformToGlobal(), Color.Red);
        }

        private void CheckLooseCondition()
        {
            if(!DebugGame.CanLoose)
            {
                return;
            }
            if (!Players[0].IsAlive && !Players[1].IsAlive)
            {
                Environment.Exit(0);
            }
        }
        
        public ViewModel GetViewModel()
        {
            return new ViewModel(this);
        }
    }
}
