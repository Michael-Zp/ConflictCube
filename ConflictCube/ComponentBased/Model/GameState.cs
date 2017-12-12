using System.Collections.Generic;
using System;
using ConflictCube.ComponentBased.Components;
using Zenseless.OpenGL;
using System.Drawing;
using ConflictCube.ComponentBased.Controller;
using OpenTK;

namespace ConflictCube.ComponentBased
{
    public class GameState
    {
        public GameObject PlayerUIs { get; set; }
        public GameObject Scene { get; set; }
        public List<Player> Players { get; private set; }

        private Transform SceneTransform = new Transform(.2f, 0, .8f, 1f);
        

        public GameState()
        {
            Scene = SceneBuilder.BuildScene(Levels.Level0, SceneTransform);
            InitializePlayers();
            InitializeUI();
        }
        

        private void InitializeUI()
        {
            PlayerUIs = new GameObject("PlayerUis", new Transform(0, 0, 1, 1), null);
            PlayerUIs.AddChild(new PlayerUI("Player0UI", Players[0], new Transform(-.8f, 0f, .2f, 1f), PlayerUIs));
            //PlayerUIs.AddChild(new PlayerUI("Player1UI", Players[1], new Transform( .8f, 0f, .2f, 1f), PlayerUIs));
        }

        public void InitializePlayers()
        {
            Players = new List<Player>();
            Material playerMat = new Material((Texture)Tilesets.PlayerSheet.Tex, Tilesets.PlayerSheet.CalcSpriteTexCoords(0), Color.Black);

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

            BoxCollider Player0Collider = new BoxCollider(new Transform(0, 0, 1, 1), false, null, CollisionType.Player);
            Players.Add(new Player("Player0", new Transform(0, 0, .06f, .06f), Player0Collider, playerMat, currentFloor, currentFloor, .015f));
            Vector2 localStartPosition = currentFloor.FindStartPosition();
            Players[0].Transform.Position = currentFloor.Transform.TransformToLocal(new Transform(localStartPosition.X, localStartPosition.Y, 0, 0)).Position;
            Scene.AddChild(Players[0]);

            /*BoxCollider Player1Collider = new BoxCollider(new Transform(0, 0, 2, 2), false, null, CollisionType.Player);
            Players.Add(new Player("Player1", new Transform(0, 0, .06f, .06f), Player1Collider, playerMat, currentFloor, currentFloor, .015f));
            Players[0].Transform.Position = currentFloor.FindStartPosition();
            Scene.AddChild(Players[0]);*/
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
            
            /*
            if(Players[0].IsAlive)
            {
                Console.WriteLine("ALIVE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
            else
            {
                Console.WriteLine("Dead");
            }
            */
            if (!Players[0].IsAlive/* && !Players[1].IsAlive*/)
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
