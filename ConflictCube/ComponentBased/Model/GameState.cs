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
        public Camera Player1UICamera = new Camera(new Transform(0, 0, 1, 1), null);
        public Camera Player2UICamera = new Camera(new Transform(0, 0, 1, 1), null);
        public Camera Player1Camera = new Camera(new Transform(0, 0, 1, 1), null);
        public Camera Player2Camera = new Camera(new Transform(.85f, 0, 1, 1), null);
        public PlayerArea Player1Area { get; set; }
        public PlayerArea Player2Area { get; set; }
        public Game Game { get; set; }
        public List<Player> Players { get; private set; }

        private Transform UiPlayer1Transform = new Transform(-.8f, 0f, .2f, 1f);
        private Transform ScenePlayer1Transform = new Transform(.15f, 0, .75f, 1f);

        private Transform UiPlayer2Transform = new Transform(.8f, 0f, .2f, 1f);
        private Transform ScenePlayer2Transform = new Transform(-.15f, 0, .75f, 1f);


        public GameState()
        {
            Game = new Game("Game", new Transform(0, 0, 1, 1));
            Player1Area = new PlayerArea("Player1Area", new Transform(-0.5f, 0, 0.5f, 1f), Game);
            Player2Area = new PlayerArea("Player2Area", new Transform( 0.5f, 0, 0.5f, 1f), Game);
            Game.AddChild(Player1Area);
            Game.AddChild(Player2Area);

            GameObject scene = SceneBuilder.BuildScene(Levels.FireIceSecondTest, ScenePlayer1Transform);
            Player1Area.AddChild(scene);
            Player1Camera.RootGameObject = scene;
            Player2Camera.RootGameObject = scene;
            
            InitializePlayers();
            InitializeUI();
        }
        

        private void InitializeUI()
        {
            GameObject player1UI = new PlayerUI("Player0UI", Players[0], UiPlayer1Transform, Player1Area);
            Player1Area.AddChild(player1UI);
            Player1UICamera.RootGameObject = player1UI;
            
            GameObject player2UI = new PlayerUI("Player0UI", Players[1], UiPlayer2Transform, Player2Area);
            Player2Area.AddChild(player2UI);
            Player2UICamera.RootGameObject = player2UI;
        }

        public void InitializePlayers()
        {
            Players = new List<Player>();
            Material playerMat = new Material((Texture)Tilesets.Instance().PlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0), Color.White);
            Material playerOrangeMat = new Material(null, null, Color.FromArgb(128, Color.Orange));
            Material playerBlueMat = new Material(null, null, Color.FromArgb(128, Color.DarkBlue));
            Material playerGhostMat = new Material((Texture)Tilesets.Instance().PlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0), Color.FromArgb(64, 255, 255, 255));

            Floor floor = (Floor)Player1Area.FindGameObjectByTypeInChildren<Floor>();

            if (floor == null)
            {
                throw new Exception("Found no active floor for player 1.");
            }
            


            //Players
            BoxCollider Player1Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, floor.CollisionGroup, CollisionType.PlayerFire);
            Players.Add(new OrangePlayer("FirePlayer", new Transform(0, 0, .06f, .06f), Player1Collider, playerMat, floor, floor, .2f, GameObjectType.PlayerFire));
            floor.AddChild(Players[0]);
            Players[0].Transform.SetPosition(floor.FindStartPosition(), WorldRelation.Global);
            Players[0].AddChild(new ColoredBox("Player0Orange", new Transform(), playerOrangeMat, Players[0]));

            BoxCollider Player2Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, floor.CollisionGroup, CollisionType.PlayerIce);
            Players.Add(new BluePlayer("IcePlayer", new Transform(0, 0, .06f, .06f), Player2Collider, playerMat, floor, floor, .2f, GameObjectType.PlayerIce));
            floor.AddChild(Players[1]);
            Players[1].Transform.SetPosition(floor.FindStartPosition(), WorldRelation.Global);
            Players[1].AddChild(new ColoredBox("Player1Blue", new Transform(), playerBlueMat, Players[1]));

            //Ghost Players
            floor.AddChild(new GhostPlayer("GhostPlayer2OnArea1", new Transform(0, 0, .06f, .06f), floor, playerGhostMat, Players[1], GameObjectType.GhostPlayer));
            floor.AddChild(new GhostPlayer("GhostPlayer1OnArea2", new Transform(0, 0, .06f, .06f), floor, playerGhostMat, Players[0], GameObjectType.GhostPlayer));
        }


        public void UpdateAll()
        {
            Game.UpdateAll();

            CheckLooseCondition();

            Player1Camera.Transform.SetPosition(new Vector2(Player1Camera.Transform.GetPosition(WorldRelation.Global).X, -Players[0].Transform.GetPosition(WorldRelation.Global).Y), WorldRelation.Global);
            Player2Camera.Transform.SetPosition(new Vector2(Player2Camera.Transform.GetPosition(WorldRelation.Global).X, -Players[1].Transform.GetPosition(WorldRelation.Global).Y), WorldRelation.Global);
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
