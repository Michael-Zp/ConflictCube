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

        private List<ICollidable> collisionGroups = new List<ICollidable>();
        
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
            collidableObjects.Clear();
            collidableObjects.AddRange(CurrentLevel.GetColliders());
            collidableObjects.AddRange(Players);
            collidableObjects.AddRange(ScreenBoundaries);
        }

        public void Update(List<Input> inputs, float diffTime)
        {
            InputManager.ExecuteInputs(inputs);

            CurrentLevel.UpdateLevel(diffTime);
            UpdateColliders();

            CheckLooseCondition();
        }

        public void MoveObject(IMoveable obj, Vector2 moveVetor)
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
            CurrentLevel.FloorOffsetPerSecond = .05f;
            CurrentLevel.StartRollingLevelOffsetSeconds = 1.0f;
            CurrentLevel.MoveObject = MoveObject;
        }

        private void CheckLooseCondition()
        {
            Console.WriteLine("Player0: " + Players[0].IsAlive + " ; Player1: " + Players[1].IsAlive);
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
