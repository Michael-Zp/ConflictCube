using ConflictCube.ComponentBased.Components;
using System.Drawing;

namespace ConflictCube.ComponentBased.Model.Components.UI
{
    public class GameOverScreen : GameObject
    {
        public TextField DeathReason;

        private const float SizePerCharacterInDeathReason = 0.04f;

        public GameOverScreen(string name, Transform transform, bool enabled = true) : base(name, transform)
        {
            AddChild(new ColoredBox("GameOverBackground", new Transform(), new Material(Color.FromArgb(245, Color.Black))));
            AddChild(new TextField("GameOverText", new Transform(0f, .4f, 1f, .3f), "Game over", Font.Instance().BloodBath));
            AddChild(new TextField("Press any key...", new Transform(0f, -.7f, 1.5f, .15f), "Press any key to start over...", Font.Instance().BloodBath));


            DeathReason = new TextField("DeathReason", new Transform(0f, -.1f, 1, .15f), "", Font.Instance().BloodBath);
            SetDeathReason();
            AddChild(DeathReason);
        }

        public void SetDeathReason(string text = "Death reason")
        {
            DeathReason.Transform.SetSize(new OpenTK.Vector2(text.Length * SizePerCharacterInDeathReason, DeathReason.Transform.GetSize(WorldRelation.Global).Y), WorldRelation.Global);
            DeathReason.Text = text;
        }
    }
}
