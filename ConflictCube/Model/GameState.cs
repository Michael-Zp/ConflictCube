using OpenTK;
using Zenseless.Geometry;
using ConflictCube.Model.Renderable;
using ConflictCube.Controller;
using System.Collections.Generic;
using System;

namespace ConflictCube.Model
{
    public class GameState
    {
        public InputManager InputManager { get; private set; }
        public Level CurrentLevel { get; set; }
        public Player Player { get; private set; }
        
        private Boundary[] Boundaries =
        {
            new Boundary(new Box2D(-1.5f, -1f,  .5f,  2f), CollisionType.LeftBoundary),
            new Boundary(new Box2D( 1f,   -1f,  .5f,  2f), CollisionType.RightBoundary),
            new Boundary(new Box2D(-1f,    1f,   2f, .5f), CollisionType.TopBoundary),
            new Boundary(new Box2D(-1f,   -1.5f, 2f, .5f), CollisionType.BottomBoundary)
        };

        public GameState()
        {
            LoadLevel(0);
            InitializePlayer();

            InputManager = new InputManager(Player);
        }

        public void Update(List<Input> inputs, float diffTime)
        {
            InputManager.ExecuteInputs(inputs);

            CurrentLevel.Floor.MoveFloorUp(CurrentLevel.FloorOffsetPerSecond * diffTime);

            CheckLooseCondition();
        }

        public void InitializePlayer()
        {
            Player = new Player(new Vector2(.1f, .1f), new Vector2(.1f, .1f), .02f);
            CurrentLevel.Floor.AddAttachedObject(Player);
            Player.SetPosition(CurrentLevel.Floor.FindStartPosition());
        }

        public void LoadLevel(int levelNumber)
        {
            CurrentLevel = LevelBuilder.LoadLevel(levelNumber);

            //Hard coded parameters. Enhance level format or even build own level format including these parameters.
            CurrentLevel.Floor.FloorSize = new Vector2(4,5);
            CurrentLevel.FloorOffsetPerSecond = .1f;
        }

        private void CheckLooseCondition()
        {
            if (!Player.IsAlive)
            {
                Environment.Exit(0);
            }
        }

        private void CheckCollisions()
        {
            CheckLevelBoundaries(Player);
        }

        private void CheckLevelBoundaries<T>(T obj) where T : RenderableObject, ICollidable 
        {
            foreach (Boundary boundary in Boundaries)
            {
                if (obj.Box.Intersects(boundary.Box))
                {
                    obj.OnCollide(boundary.CollisionType);
                }
            }

        }

        public ViewModel GetViewModel()
        {
            return new ViewModel(this);
        }
    }
}
