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
        public List<Player> Players { get; private set; }

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
            LoadLevel(2);
            InitializePlayers();

            InputManager = new InputManager(this, Players);

            InitializeColliders();
        }

        private void InitializeColliders()
        {
            collidableObjects.AddRange(CurrentLevel.GetColliders());
            collidableObjects.AddRange(Players);
            collidableObjects.AddRange(Boundaries);
        }

        public void Update(List<Input> inputs, float diffTime)
        {
            InputManager.ExecuteInputs(inputs);

            CurrentLevel.UpdateLevel(diffTime);

            CheckLooseCondition();
        }

        public void MoveObject<T>(T obj, Vector2 moveVetor) where T : RenderableObject, IMoveable
        {
            if(obj is Player)
            {
                Player plr = obj as Player;
                if(!plr.IsAlive)
                {
                    return;
                }
            }

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

        public void InitializePlayers()
        {
            Players = new List<Player>();

            Players.Add(new Player(new Vector2(.08f, .08f), new Vector2(.1f, .1f), .01f));
            CurrentLevel.FloorLeft.AddAttachedObject(Players[0]);
            Players[0].SetPosition(CurrentLevel.FloorLeft.FindStartPosition());


            Players.Add(new Player(new Vector2(.08f, .08f), new Vector2(.1f, .1f), .01f));
            CurrentLevel.FloorRight.AddAttachedObject(Players[1]);
            Players[1].SetPosition(CurrentLevel.FloorRight.FindStartPosition());
        }

        public void LoadLevel(int levelNumber)
        {
            CurrentLevel = LevelBuilder.LoadLevel(levelNumber);

            //Hard coded parameters. Enhance level format or even build own level format including these parameters.
            CurrentLevel.FloorLeft.FloorSize = new Vector2(10, 10);
            CurrentLevel.FloorMiddle.FloorSize = new Vector2(1, 10);
            CurrentLevel.FloorRight.FloorSize = new Vector2(10, 10);
            CurrentLevel.FloorOffsetPerSecond = .1f;
            CurrentLevel.StartRollingLevelOffsetSeconds = 3.0f;
        }

        private void CheckLooseCondition()
        {
            if (!Players[0].IsAlive && !Players[1].IsAlive)
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
