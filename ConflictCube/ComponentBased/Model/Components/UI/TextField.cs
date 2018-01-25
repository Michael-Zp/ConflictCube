using ConflictCube.ComponentBased.Components;
using static ConflictCube.ComponentBased.View.ZenselessWrapper;

namespace ConflictCube.ComponentBased.Model.Components.UI
{
    public class TextField : GameObject
    {
        public string Text { get; set; }
        public MyTextureFont Font { get; set; }

        public TextField(string name, Transform transform, string text, MyTextureFont font, GameObject parent) : base(name, transform, parent, GameObjectType.Text)
        {
            Text = text;
            Font = font;
        }


    }
}
