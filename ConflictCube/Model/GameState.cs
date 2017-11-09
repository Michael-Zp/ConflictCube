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

        private List<ICollidable> collidableObjects = new List<ICollidable>();
        
        private Boundary[] Boundaries =
        {
            new Boundary(new Box2D(-1.5f, -1f,  .5f,  2f), CollisionType.LeftBoundary),
            new Boundary(new Box2D( 1f,   -1f,  .5f,  2f), CollisionType.RightBoundary),
            new Boundary(new Box2D(-1f,    1f,   2f, .5f), CollisionType.TopBoundary),
            new Boundary(new Box2D(-1f,   -1.5f, 2f, .5f), CollisionType.BottomBoundary)
        };

        public GameState()
        {
            LoadLevel(1);
            InitializePlayer();

            InputManager = new InputManager(this, Player);

            InitializeColliders();
        }

        private void InitializeColliders()
        {
            collidableObjects.AddRange(CurrentLevel.GetColliders());
            collidableObjects.Add(Player);
            collidableObjects.AddRange(Boundaries);
        }

        public void Update(List<Input> inputs, float diffTime)
        {
            InputManager.ExecuteInputs(inputs);

            CurrentLevel.MoveFloorsUp(diffTime);

            CheckLooseCondition();
        }

        public void MoveObject<T>(T obj, Vector2 moveVetor) where T : RenderableObject, IMoveable
        {
            if(obj is ICollidable)
            {
                obj.Move(moveVetor);
                CheckCollisions((ICollidable)obj, moveVetor);
            }
            else
            {
                obj.Move(moveVetor);
            }
        }

        public void InitializePlayer()
        {
            Player = new Player(new Vector2(.08f, .08f), new Vector2(.1f, .1f), .02f);
            CurrentLevel.FloorLeft.AddAttachedObject(Player);
            Player.SetPosition(CurrentLevel.FloorLeft.FindStartPosition());
        }

        public void LoadLevel(int levelNumber)
        {
            CurrentLevel = LevelBuilder.LoadLevel(levelNumber);

            //Hard coded parameters. Enhance level format or even build own level format including these parameters.
            CurrentLevel.FloorLeft.FloorSize = new Vector2(10,30);
            CurrentLevel.FloorMiddle.FloorSize = new Vector2(1, 30);
            CurrentLevel.FloorRight.FloorSize = new Vector2(10, 30);
            CurrentLevel.FloorOffsetPerSecond = .1f;
        }

        private void CheckLooseCondition()
        {
            if (!Player.IsAlive)
            {
                Environment.Exit(0);
            }
        }

        public void CheckCollisions(ICollidable collider, Vector2 movement)
        {
            foreach(ICollidable other in collidableObjects)
            {
                if(other != collider)
                {
                    CheckCollision(collider, other, movement);
                }
            }
        }

        public void CheckCollision(ICollidable obj, ICollidable other, Vector2 movement)
        {
            if (obj.CollisionBox.Intersects(other.CollisionBox))
            {
                obj.OnCollide(other.CollisionType, other, movement);
            }
        }

        public ViewModel GetViewModel()
        {
            return new ViewModel(this);
        }
    }
}
