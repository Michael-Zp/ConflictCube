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
        public Camera Player2Camera = new Camera(new Transform(0, 0, 1, 1), null);
        public PlayerArea Player1Area { get; set; }
        public PlayerArea Player2Area { get; set; }
        public Game Game { get; set; }
        public List<Player> Players { get; private set; }

        private Transform UiTransform = new Transform(-.8f, 0f, .2f, 1f);
        private Transform SceneTransform = new Transform(.2f, 0, .8f, 1f);


        public GameState()
        {
            Game = new Game("Game", new Transform(0, 0, 1, 1));
            Player1Area = new PlayerArea("Player1Area", new Transform(-0.5f, 0, 0.5f, 1f), Game);
            Player2Area = new PlayerArea("Player2Area", new Transform( 0.5f, 0, 0.5f, 1f), Game);
            Game.AddChild(Player1Area);
            Game.AddChild(Player2Area);

            GameObject player1Scene = SceneBuilder.BuildScene(Levels.Level2, SceneTransform);
            Player1Area.AddChild(player1Scene);
            Player1Camera.RootGameObject = player1Scene;

            GameObject player2Scene = SceneBuilder.BuildScene(Levels.Level2, SceneTransform);
            Player2Area.AddChild(player2Scene);
            Player2Camera.RootGameObject = player2Scene;


            InitializePlayers();
            InitializeUI();
        }
        

        private void InitializeUI()
        {
            GameObject player1UI = new PlayerUI("Player0UI", Players[0], UiTransform, Player1Area);
            Player1Area.AddChild(player1UI);
            Player1UICamera.RootGameObject = player1UI;
            
            GameObject player2UI = new PlayerUI("Player0UI", Players[1], UiTransform, Player2Area);
            Player2Area.AddChild(player2UI);
            Player2UICamera.RootGameObject = player2UI;
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

            List<Floor> allFloors = new List<Floor>();
            allFloors.Add(Player1Floor);
            allFloors.Add(Player2Floor);


            //Players
            BoxCollider Player1Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, Player1Floor.CollisionGroup, CollisionType.Player1);
            Players.Add(new Player("Player0", new Transform(0, 0, .06f, .06f), Player1Collider, playerMat, Player1Floor, 0, allFloors, .2f, GameObjectType.Player1));
            Player1Floor.AddChild(Players[0]);
            Players[0].Transform.SetPosition(Player1Floor.FindStartPosition(), WorldRelation.Global);
            
            //hier pickable initialitsieren player1
            Transform picSpeedPotion = Player1Floor.GetBoxAtGridPosition(new Vector2(3, 4));
            picSpeedPotion.SetSize(picSpeedPotion.GetSize(WorldRelation.Global) / 2, WorldRelation.Global);
            BoxCollider colliderPicSpeedPotion = new BoxCollider(new Transform(0, 0, 1, 1), true, Player1Floor.CollisionGroup, CollisionType.PickableSpeedPotion);
            Material matPicSpeedPotion = new Material(null, null, Color.Yellow);
            Player1Floor.AddChild(new Pickable("speedpotion", Player1Floor.Transform.TransformToLocal(picSpeedPotion), Player1Floor, colliderPicSpeedPotion, matPicSpeedPotion));
            

            BoxCollider Player2Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, Player2Floor.CollisionGroup, CollisionType.Player2);
            Players.Add(new Player("Player1", new Transform(0, 0, .06f, .06f), Player2Collider, playerMat, Player2Floor, 1, allFloors, .2f, GameObjectType.Player2));
            Player2Floor.AddChild(Players[1]);
            Players[1].Transform.SetPosition(Player2Floor.FindStartPosition(), WorldRelation.Global);

            //hier pickable initialitsieren player2
            Transform picBlock = Player2Floor.GetBoxAtGridPosition(new Vector2(3, 4));
            picBlock.SetSize(picBlock.GetSize(WorldRelation.Global) / 2, WorldRelation.Global);
            BoxCollider colliderPicBlock = new BoxCollider(new Transform(0, 0, 1, 1), true, Player2Floor.CollisionGroup, CollisionType.PickableBlock);
            Material matPicBlock = new Material(null, null, Color.White);
            Player2Floor.AddChild(new Pickable("speedpotion", Player2Floor.Transform.TransformToLocal(picBlock), Player2Floor, colliderPicBlock, matPicBlock));

            //Ghost Players
            Player1Floor.AddChild(new GhostPlayer("GhostPlayer2OnArea1", new Transform(0, 0, .06f, .06f), Player1Floor, playerGhostMat, Players[1], GameObjectType.GhostPlayer));
            Player2Floor.AddChild(new GhostPlayer("GhostPlayer1OnArea2", new Transform(0, 0, .06f, .06f), Player2Floor, playerGhostMat, Players[0], GameObjectType.GhostPlayer));
        }


        public void UpdateAll()
        {
            Game.UpdateAll();

            CheckLooseCondition();

            Player1Camera.Transform.SetPosition(new Vector2(0, -Players[0].Transform.GetPosition(WorldRelation.Global).Y), WorldRelation.Global);
            Player2Camera.Transform.SetPosition(new Vector2(0, -Players[1].Transform.GetPosition(WorldRelation.Global).Y), WorldRelation.Global);
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
