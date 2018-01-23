﻿using ConflictCube.ComponentBased.Components;
using System.Drawing;

namespace ConflictCube.ComponentBased.Model.Components.UI
{
    public class GameWonScreen : GameObject
    {
        public TextField DeathReason;

        private const float SizePerCharacterInDeathReason = 0.04f;

        public GameWonScreen(string name, Transform transform, bool enabled = true) : base(name, transform)
        {
            AddChild(new ColoredBox("GameWonBackground", new Transform(), new Material(Color.FromArgb(245, Color.White))));
            AddChild(new TextField("GameWonText", new Transform(0f, .4f, 1.5f, .3f), "Level finished", Font.Instance().NormalFont));
            AddChild(new TextField("Press any key...", new Transform(0f, -.7f, 1.5f, .15f), "Press any key to show menu.", Font.Instance().NormalFont));
        }
    }
}
