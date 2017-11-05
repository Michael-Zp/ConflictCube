using ConflictCube.Model.Renderable;
using ConflictCube.Model.Tiles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictCube
{
    public class GameState
    {
        private GameView View;

        public Level CurrentLevel { get; set; }
        public Player Player { get; private set; }

        public GameState(GameView view)
        {
            this.View = view;

            InitializePlayer();
        }

        private void InitializePlayer()
        {
            TilesetTile tilesetTile;

            Player.PlayerTileset.TilesetTiles.TryGetValue(Player.DefaultTileType, out tilesetTile);

            this.Player = new Player(tilesetTile, new Vector2(.1f, .1f), new Vector2(.1f, .1f));
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
            CurrentLevel.Floor.FloorSize = new OpenTK.Vector2(4,5);
            CurrentLevel.FloorOffsetPerSecond = .1f;
        }

        public void CloseGame()
        {
            View.CloseWindow();
        }

        public void NextFrame(float diffTime)
        {
            CurrentLevel.Floor.MoveFloorUp(CurrentLevel.FloorOffsetPerSecond * diffTime);
        }
    }
}
