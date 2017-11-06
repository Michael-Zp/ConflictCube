using System;
using ConflictCube.Model.Tiles;
using OpenTK;
using Zenseless.Geometry;
using ConflictCube.Model.Renderable;

namespace ConflictCube.Model
{
    public class GameState
    {
        private GameView View;

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


        public GameState(GameView view)
        {
            View = view;
            InputManager = new InputManager(View);
        }

        public void InitializePlayer()
        {
            TilesetTile tilesetTile;

            Player.PlayerTileset.TilesetTiles.TryGetValue(Player.DefaultTileType, out tilesetTile);

            Player = new Player(tilesetTile, new Vector2(.1f, .1f), new Vector2(.1f, .1f), .02f);
            CurrentLevel.Floor.AddAttachedObject(Player);
            InputManager.Player = Player;
            Player.SetPosition(CurrentLevel.Floor.FindStartPosition());
        }

        public void UpdateView()
        {
            View.ClearScreen();
            View.SetLevel(CurrentLevel);
            View.AddPlayer(Player);
        }

        public void LoadLevel(int levelNumber)
        {
            CurrentLevel = LevelBuilder.LoadLevel(levelNumber);

            //Hard coded parameters. Enhance level format or even build own level format including these parameters.
            CurrentLevel.Floor.FloorSize = new Vector2(4,5);
            CurrentLevel.FloorOffsetPerSecond = .1f;
        }

        public void NextFrame(float diffTime)
        {
            CurrentLevel.Floor.MoveFloorUp(CurrentLevel.FloorOffsetPerSecond * diffTime);

            CheckCollisions();

            CheckLooseCondition();
        }

        private void CheckLooseCondition()
        {
            if (!Player.IsAlive)
            {
                InputManager.CloseGame();
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
    }
}
