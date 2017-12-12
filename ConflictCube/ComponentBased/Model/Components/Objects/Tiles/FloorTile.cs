namespace ConflictCube.ComponentBased.Components.Objects.Tiles
{
    public class FloorTile : GameObject
    {
        public int Row { get; private set; }
        public int Column { get; private set; }

        public FloorTile(int row, int column, string name, Transform transform, Material material, GameObject parent, GameObjectType type) : base(name, transform, parent, type)
        {
            Row = row;
            Column = column;

            AddComponent(material);
        }

        public override GameObject Clone()
        {
            FloorTile newGameObject = (FloorTile)base.Clone();

            newGameObject.Row = Row;
            newGameObject.Column = Column;

            return newGameObject;
        }
    }
}
