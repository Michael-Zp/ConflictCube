using ConflictCube.ComponentBased.Components;

namespace ConflictCube.ComponentBased.Model.Components.UI
{
    public class TextField : GameObject
    {
        public string Text { get; set; }

        public TextField(string name, Transform transform, string text) : base(name, transform, null, GameObjectType.Text)
        {
            Text = text;
        }


    }
}
