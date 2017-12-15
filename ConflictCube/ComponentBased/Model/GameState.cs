using System.Collections.Generic;
using System;
using ConflictCube.ComponentBased.Components;
using Zenseless.OpenGL;
using System.Drawing;
using ConflictCube.ComponentBased.Controller;
using OpenTK;
using ConflictCube.ComponentBased.Model.Components.Objects;

namespace ConflictCube.ComponentBased
{
    public class GameState
    {
        public GameObject Game { get; set; }
        public PlayerArea Player1Area { get; set; }
        public PlayerArea Player2Area { get; set; }
        public List<Player> Players { get; private set; }

        private Transform UiTransform = new Transform(-.8f, 0f, .2f, 1f);
        private Transform SceneTransform = new Transform(.2f, 0, .8f, 1f);


        public GameState()
        {
            Game = new GameObject("Game", new Transform(0, 0, 1, 1), null);
            Player1Area = new PlayerArea("Player1Area", new Transform(-0.5f, 0, 0.5f, 1f), Game);
            Player2Area = new PlayerArea("Player2Area", new Transform( 0.5f, 0, 0.5f, 1f), Game);
            Game.AddChild(Player1Area);
            Game.AddChild(Player2Area);

            Player1Area.AddChild(SceneBuilder.BuildScene(Levels.Level0, SceneTransform));
            Player2Area.AddChild(SceneBuilder.BuildScene(Levels.Level0, SceneTransform));
            InitializePlayers();
            InitializeUI();
        }
        

        private void InitializeUI()
        {
            Player1Area.AddChild(new PlayerUI("Player0UI", Players[0], UiTransform, Player1Area));
            Player2Area.AddChild(new PlayerUI("Player0UI", Players[1], UiTransform, Player2Area));
        }

        public void InitializePlayers()
        {
            Players = new List<Player>();
            Material playerMat = new Material((Texture)Tilesets.Instance().PlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0), Color.White);
            Material playerGhostMat = new Material((Texture)Tilesets.Instance().PlayerSheet.Tex, Tilesets.Instance().PlayerSheet.CalcSpriteTexCoords(0), Color.FromArgb(64, 255, 255, 255));

            Floor Player1Floor = (Floor)Player1Area.FindGameObjectByTypeInChildren<Floor>();
            Floor Player2Floor = (Floor)Player2Area.FindGameObjectByTypeInChildren<Floor>();

            if (Player1Floor == null)
            {
                throw new Exception("Found no active floor for player 1.");
            }
            if (Player2Floor == null)
            {
                throw new Exception("Found no active floor for player 2.");
            }


            //Players
            BoxCollider Player1Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, Player1Floor.CollisionGroup, CollisionType.Player1);
            Players.Add(new Player("Player0", new Transform(0, 0, .06f, .06f), Player1Collider, playerMat, Player1Floor, Player1Floor, .015f, GameObjectType.Player1));
            Player1Area.AddChild(Players[0]);
            Vector2 globalStartPosition = Player1Floor.FindStartPosition();
            Players[0].Transform.Position = Player1Area.Transform.TransformToLocal(new Transform(globalStartPosition.X, globalStartPosition.Y, 0, 0)).Position;

            BoxCollider Player2Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, Player2Floor.CollisionGroup, CollisionType.Player2);
            Players.Add(new Player("Player1", new Transform(0, 0, .06f, .06f), Player2Collider, playerMat, Player2Floor, Player2Floor, .015f, GameObjectType.Player2));
            Player2Area.AddChild(Players[1]);
            globalStartPosition = Player2Floor.FindStartPosition();
            Players[1].Transform.Position = Player2Area.Transform.TransformToLocal(new Transform(globalStartPosition.X, globalStartPosition.Y, 0, 0)).Position;


            //Ghost Players
            Player1Area.AddChild(new GhostPlayer("GhostPlayer2OnArea1", new Transform(0, 0, .06f, .06f), Player1Area, playerGhostMat, Players[1], GameObjectType.GhostPlayer));
            Player2Area.AddChild(new GhostPlayer("GhostPlayer1OnArea2", new Transform(0, 0, .06f, .06f), Player2Area, playerGhostMat, Players[0], GameObjectType.GhostPlayer));
        }


        public void UpdateAll()
        {
            Game.UpdateAll();

            CheckLooseCondition();
        }

        private void CheckLooseCondition()
        {
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
