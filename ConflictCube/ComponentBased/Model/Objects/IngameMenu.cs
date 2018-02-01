using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Model.Components.UI;
using System;

namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public class IngameMenu : GameObject
    {
        private GameObject menu;

        public IngameMenu(string name, GameObject parent, IBuildMenu buildMenu, bool enabled = true) : base(name, new Transform(), parent, enabled)
        {
            menu = new GameObject("IngameMenuHolder", new Transform(), this, false);

            new ColoredBox("Background", new Transform(), new Material(System.Drawing.Color.FromArgb(250, System.Drawing.Color.Black)), menu);

            ButtonGroup btnGroup = new ButtonGroup("Buttons", new Transform(), menu);

            Action resume = new Action(() => { Time.Time.TimeScale = 1; menu.Enabled = false; });
            Action backToMenu = new Action(() => { Time.Time.TimeScale = 1; buildMenu.BuildMenu(); });

            btnGroup.AddButton(new Transform(0f, .3f, .8f, .2f), new TextField("Resume", new Transform(0f, .3f, .6f, .3f), "Resume", Font.Instance().NormalFont, btnGroup), resume);
            btnGroup.AddButton(new Transform(0f, -.3f, .8f, .2f), new TextField("BackToMenu", new Transform(0f, -.3f, .9f, .3f), "Back to menu", Font.Instance().NormalFont, btnGroup), backToMenu);
        }

        public override void OnUpdate()
        {
            if(Input.OnButtonDown(OpenTK.Input.Key.Escape))
            {
                if(menu.EnabledSelf)
                {
                    menu.Enabled = false;
                    Time.Time.TimeScale = 1;
                }
                else
                {
                    menu.Enabled = true;
                    Time.Time.TimeScale = 0;
                }
            }
        }
    }
}
