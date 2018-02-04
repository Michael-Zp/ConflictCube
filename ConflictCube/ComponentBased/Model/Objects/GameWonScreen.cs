using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.Animators;
using System.Drawing;

namespace ConflictCube.ComponentBased.Model.Components.UI
{
    public class GameWonScreen : GameObject
    {
        public TextField DeathReason;

        private const float SizePerCharacterInDeathReason = 0.04f;

        public GameWonScreen(string name, Transform transform, GameObject parent, bool enabled = true) : base(name, transform, parent, enabled)
        {
            Material bgMat = new Material(Color.FromArgb(220, Color.White));
            new ColoredBox("GameWonBackground", new Transform(), bgMat, this);
            new TextField("GameWonText", new Transform(0f, .4f, 1.5f, .3f), "Level finished", Font.Instance().NormalFont, this);
            new TextField("Press any key...", new Transform(0f, -.7f, 1.5f, .15f), "Press any key to show menu.", Font.Instance().NormalFont, this);


            MaterialAnimator animator = new MaterialAnimator(bgMat);
            animator.AddKeyframe(0, 100);
            animator.AddKeyframe(1, 220);
            AddComponent(animator);
        }
    }
}
