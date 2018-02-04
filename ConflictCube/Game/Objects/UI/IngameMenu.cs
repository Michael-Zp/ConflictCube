using ConflictCube.Objects;
using ConflictCube.SceneBuilders;
using Engine.Components;
using Engine.Inputs;
using Engine.Time;
using System;

namespace ConflictCube.Objects
{
    public class IngameMenu : GameObject
    {
        private GameObject menu;

        public IngameMenu(string name, GameObject parent, IBuildMenu buildMenu, bool enabled = true) : base(name, new Transform(), parent, enabled)
        {
            menu = new GameObject("IngameMenuHolder", new Transform(), this, false);

            new ColoredBox("Background", new Transform(), new Material(System.Drawing.Color.FromArgb(250, System.Drawing.Color.Black)), menu);

            ButtonGroup btnGroup = new ButtonGroup("Buttons", new Transform(), menu);

            Action resume = new Action(() => { Time.TimeScale = 1; menu.Enabled = false; });
            Action backToMenu = new Action(() => { Time.TimeScale = 1; buildMenu.BuildMenu(); });

            btnGroup.AddButton(new Transform(0f, .3f, .8f, .2f), "Resume", resume);
            btnGroup.AddButton(new Transform(0f, -.3f, .8f, .2f),"Back to menu", backToMenu);
        }

        public override void OnUpdate()
        {
            if(Input.OnButtonDown(OpenTK.Input.Key.Escape))
            {
                if(menu.EnabledSelf)
                {
                    menu.Enabled = false;
                    Time.TimeScale = 1;
                }
                else
                {
                    menu.Enabled = true;
                    Time.TimeScale = 0;
                }
            }
        }
    }
}
