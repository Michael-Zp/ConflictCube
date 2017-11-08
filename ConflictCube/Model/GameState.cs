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

        private List<ICollidable> collidables = new List<ICollidable>();
        
        

        public GameState()
        {
            LoadLevel(0);
            InitializePlayer();

            InputManager = new InputManager(Player);

            AddCollidables();
        }

        private void AddCollidables()
        {   
            collidables.Add(new Boundary(new Box2D(-1.5f, -1f, .5f, 2f), CollisionType.LeftBoundary));
            collidables.Add(new Boundary(new Box2D(1f, -1f, .5f, 2f), CollisionType.RightBoundary));
            collidables.Add(new Boundary(new Box2D(-1f, 1f, 2f, .5f), CollisionType.TopBoundary));
            collidables.Add(new Boundary(new Box2D(-1f, -1.5f, 2f, .5f), CollisionType.BottomBoundary));
            collidables.AddRange(CurrentLevel.Floor.GetColliders());
            collidables.Add(Player);
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

        public void MoveObject(RenderableObject obj, Vector2 moveVector)
        {
            if(!(obj is IMoveable))
            {
                return;
            }

            IMoveable moveable = (IMoveable)obj;
            moveable.Move(moveVector);

            if (moveable is ICollidable)
            {

            }
        }

        private void CheckLooseCondition()
        {
            if (!Player.IsAlive)
            {
                Environment.Exit(0);
            }
        }

        private void CheckCollisions(RenderableObject obj)
        {
            foreach(RenderableObject collidable in collidables)
            {
                CheckCollision(Player, collidable);
            }
        }

        private void CheckCollision<T, K>(T obj, K other) where T : RenderableObject, ICollidable where K : RenderableObject, ICollidable
        {
                if (obj.Box.Intersects(other.Box))
                {
                    obj.OnCollide(other.CollisionType, other);
                }
        }

        public ViewModel GetViewModel()
        {
            return new ViewModel(this);
        }
    }
}
