using Engine.Components;
using static Engine.View.ZenselessWrapper;

namespace Engine.UI
{
    public class TextField : GameObject
    {
        public string Text { get; set; }
        public MyTextureFont Font { get; set; }

        public TextField(string name, Transform transform, string text, MyTextureFont font, GameObject parent, string type = "Text") : base(name, transform, parent, type)
        {
            Text = text;
            Font = font;
        }
    }
}
