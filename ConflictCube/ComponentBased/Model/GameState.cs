using System.Collections.Generic;
using System;
using ConflictCube.ComponentBased.Components;
using Zenseless.OpenGL;
using System.Drawing;
using ConflictCube.ComponentBased.Controller;

namespace ConflictCube.ComponentBased
{
    public class GameState
    {
        public GameObject PlayerUIs { get; set; }
        public GameObject Scene { get; set; }
        public List<Player> Players { get; private set; }


        private Boundary[] ScreenBoundaries =
        {
        };

        public GameState()
        {
            Scene = SceneBuilder.BuildScene(Levels.Level0);
            InitializeBoundaries();
            InitializePlayers();
            InitializeUI();
        }

        private void InitializeBoundaries()
        {
            GameObject Boundaries = new GameObject("Boundaries", new Transform(0, 0, 2, 2), Scene);
            Boundaries.AddChild(new Boundary("TopBoundary", new Transform(0, 1.25f, 2f, .5f), null, null, CollisionType.TopBoundary));
            Boundaries.AddChild(new Boundary("BottomBoundary", new Transform(0, -1.25f, 2f, .5f), null, null, CollisionType.BottomBoundary));
            Boundaries.AddChild(new Boundary("RightBoundary", new Transform(1.25f, 0f, .5f, 2f), null, null, CollisionType.RightBoundary));
            Boundaries.AddChild(new Boundary("LeftBoundary", new Transform(-1.25f, 0f, .5f, 2f), null, null, CollisionType.LeftBoundary));
        }

        private void InitializeUI()
        {
            PlayerUIs = new GameObject("PlayerUis", new Transform(0, 0, 2, 2), null);
            PlayerUIs.AddChild(new PlayerUI("Player0UI", Players[0], new Transform(-1f, -1f, .2f, 2f), PlayerUIs));
            PlayerUIs.AddChild(new PlayerUI("Player1UI", Players[1], new Transform(.8f, -1f, .2f, 2f), PlayerUIs));
        }

        public void InitializePlayers()
        {
            Players = new List<Player>();
            Material playerMat = new Material((Texture)Tilesets.PlayerSheet.Tex, Tilesets.PlayerSheet.CalcSpriteTexCoords(0), Color.White);

            Floor currentFloor = null;
            foreach(GameObject child in Scene.Children)
            {
                if(child is Floor)
                {
                    currentFloor = (Floor)child;
                }
            }

            if(currentFloor == null)
            {
                throw new Exception("Found no active floor in the scene");
            }

            BoxCollider Player0Collider = new BoxCollider(new Transform(0, 0, 2, 2), false, null, CollisionType.Player);
            Players.Add(new Player("Player0", new Transform(0, 0, .06f, .06f), Player0Collider, playerMat, currentFloor, currentFloor, .015f));
            Players[0].Transform.Position = currentFloor.FindStartPosition();

            BoxCollider Player1Collider = new BoxCollider(new Transform(0, 0, 2, 2), false, null, CollisionType.Player);
            Players.Add(new Player("Player1", new Transform(0, 0, .06f, .06f), Player1Collider, playerMat, currentFloor, currentFloor, .015f));
            Players[0].Transform.Position = currentFloor.FindStartPosition();
        }

        public void UpdateAll()
        {
            Scene.UpdateAll();
            PlayerUIs.UpdateAll();

            CheckLooseCondition();
        }

        private void CheckLooseCondition()
        {
            //Console.WriteLine("Player0: " + Players[0].IsAlive + " ; Player1: " + Players[1].IsAlive);
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
