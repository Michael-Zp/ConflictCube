using OpenTK;
using Zenseless.Geometry;
using ConflictCube.Controller;
using System.Collections.Generic;
using System;
using ConflictCube.Model.Collision;

namespace ConflictCube.Model
{
    public class GameState
    {
        public InputManager InputManager { get; private set; }
        public Level CurrentLevel { get; set; }
        public List<Player> Players { get; private set; }

        private List<CollisionGroup> CollisionGroups = new List<CollisionGroup>();
        
        private Boundary[] ScreenBoundaries =
        {
            new Boundary(new Box2D(-1f,    1f,   2f, .5f), CollisionType.TopBoundary),
            new Boundary(new Box2D(-1f,   -1.5f, 2f, .5f), CollisionType.BottomBoundary)
        };

        public GameState()
        {
            LoadLevel(1);
            InitializePlayers();

            InputManager = new InputManager(this, Players);

            UpdateColliders();
        }

        private void UpdateColliders()
        {
            CollisionGroups.Clear();

            CollisionGroup group = new CollisionGroup();
            group.CollidersInGroup.AddRange(CurrentLevel.GetColliders());
            group.CollidersInGroup.AddRange(Players);
            group.CollidersInGroup.AddRange(ScreenBoundaries);

            CollisionGroups.Add(group);
        }

        public void Update(List<Input> inputs, float diffTime)
        {
            InputManager.ExecuteInputs(inputs);

            CurrentLevel.UpdateLevel(diffTime);
            UpdateColliders();

            foreach (CollisionGroup group in CollisionGroups)
            {
                group.MoveAllObjects();
            }

            CheckLooseCondition();
        }
        
        public void InitializePlayers()
        {
            Players = new List<Player>();
            
            Players.Add(new Player(new Vector2(.06f, .06f), new Vector2(.1f, .1f), .01f));
            CurrentLevel.FloorLeft.AddAttachedObject(Players[0], CurrentLevel.ScaleMatrix);
            Players[0].SetPosition(CurrentLevel.FindStartPosition(FloorArea.Left));


            Players.Add(new Player(new Vector2(.06f, .06f), new Vector2(.1f, .1f), .01f));
            CurrentLevel.FloorRight.AddAttachedObject(Players[1], CurrentLevel.ScaleMatrix);
            Players[1].SetPosition(CurrentLevel.FindStartPosition(FloorArea.Right));
        }

        public void LoadLevel(int levelNumber)
        {
            CurrentLevel = LevelBuilder.LoadLevel(levelNumber);

            //Hard coded parameters. Enhance level format or even build own level format including these parameters.
            CurrentLevel.FloorOffsetPerSecond = .0f;
            CurrentLevel.StartRollingLevelOffsetSeconds = 1.0f;
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
