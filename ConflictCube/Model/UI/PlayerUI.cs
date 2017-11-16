using ConflictCube.Model.Renderable;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.Geometry;
using OpenTK;
using ConflictCube.Model.Tiles;

namespace ConflictCube.Model.UI
{
    public class PlayerUI : RenderableLayer
    {
        public Player Player { get; private set; }
        

        public PlayerUI(Player player, Box2D areaOfLayer) : base(new List<RenderableObject>(), new List<RenderableLayer>(), areaOfLayer)
        {
            Player = player;


            RenderableLayer bgLayer = new RenderableLayer(new List<RenderableObject>(), new List<RenderableLayer>(), new Box2D(-1, -1, 2, 2));
            SubLayers.Add(bgLayer);


            bgLayer.AddObjectsToRender(new Canvas(new Box2D(-1, -1, 2, 2), TileType.Background, Color.White));


            RenderableLayer fgLayer = new RenderableLayer(new List<RenderableObject>(), new List<RenderableLayer>(), new Box2D(-1, -1, 2, 2));
            UIText playerText = new UIText(new Box2D(-.8f, -2, 1.6f, .3f), TileType.Text, "Player" + player.ID);

            fgLayer.AddObjectsToRender(playerText);
        }


    }


    public class Canvas : RenderableObject
    {
        public Color Color { get; set; }

        public Canvas(Box2D box, TileType type, Color color) : base(box, type)
        {
            Color = color;
        }

        public override void OnBoxChanged()
        {}

        public override void SetPosition(Vector2 pos)
        {}
    }

    public class UIText : RenderableObject
    {
        public string Text { get; set; }

        public UIText(Box2D box, TileType type, string text) : base(box, type)
        {
            Text = text;
        }

        public override void OnBoxChanged()
        {}

        public override void SetPosition(Vector2 pos)
        {
            Box.CenterX = pos.X;
            Box.CenterY = pos.Y;
        }
    }
}
