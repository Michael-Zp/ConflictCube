using ConflictCube.Model.Tiles;
using OpenTK;

namespace ConflictCube.Model
{
    public class GameState
    {
        private GameView View;

        public InputManager InputManager { get; private set; }
        public Level CurrentLevel { get; set; }
        public Player Player { get; private set; }

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
        }
    }
}
