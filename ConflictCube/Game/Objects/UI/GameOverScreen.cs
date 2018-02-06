using Engine.Components;
using Engine.UI;
using System.Drawing;

namespace ConflictCube.Objects
{
    public class GameOverScreen : GameObject
    {
        public TextField DeathReason;

        private const float SizePerCharacterInDeathReason = 0.04f;

        public GameOverScreen(string name, Transform transform, GameObject parent, bool enabled = true) : base(name, transform, parent, enabled)
        {
            Material bgMat = new Material(Color.FromArgb(245, Color.Black));
            new ColoredBox("GameOverBackground", new Transform(), bgMat, this);
            new TextField("GameOverText", new Transform(0f, .4f, 1f, .3f), "Game over", Engine.UI.Font.Instance().BloodBath, this);
            new TextField("Press any key...", new Transform(0f, -.7f, 1.5f, .15f), "Press any key to start over...", Engine.UI.Font.Instance().BloodBath, this);

            MaterialAnimator animator = new MaterialAnimator(bgMat);
            animator.AddKeyframe(0, 125);
            animator.AddKeyframe(1, 245);
            AddComponent(animator);


            DeathReason = new TextField("DeathReason", new Transform(0f, -.1f, 1, .15f), "", Engine.UI.Font.Instance().BloodBath, this);
            SetDeathReason();
        }

        public void SetDeathReason(string text = "Death reason")
        {
            DeathReason.Transform.SetSize(new OpenTK.Vector2(text.Length * SizePerCharacterInDeathReason, DeathReason.Transform.GetSize(WorldRelation.Global).Y), WorldRelation.Global);
            DeathReason.Text = text;
        }
    }
}
