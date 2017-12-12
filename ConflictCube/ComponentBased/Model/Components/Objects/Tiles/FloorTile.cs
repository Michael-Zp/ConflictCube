using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Components.Objects.Tiles
{
    public class FloorTile : GameObject
    {
        public static Dictionary<GameObjectType, Material> FloorTileMaterials = new Dictionary<GameObjectType, Material>();

        public int Row { get; private set; }
        public int Column { get; private set; }

        public FloorTile(int row, int column, string name, Transform transform, GameObject parent, GameObjectType type) : base(name, transform, parent, type)
        {
            Row = row;
            Column = column;

            AddMaterialOnCreate();
            AddColliderOnCreate();
        }

        private void AddMaterialOnCreate()
        {
            FloorTileMaterials.TryGetValue(Type, out Material material);

            AddComponent(material);
        }

        private void AddColliderOnCreate()
        {
            if (Type == GameObjectType.Wall)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, null, CollisionType.Wall));
            }
            else if (Type == GameObjectType.Hole)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .8f, .8f), false, null, CollisionType.Hole));
            }
            else if (Type == GameObjectType.Finish)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), true, null, CollisionType.Finish));
            }
        }

        public void ChangeFloorTile(GameObjectType TypeToTransformTo)
        {
            if(TypeToTransformTo == Type)
            {
                return;
            }

            RemoveComponent<Material>();
            RemoveComponent<Collider>();

            Type = TypeToTransformTo;

            AddMaterialOnCreate();
            AddColliderOnCreate();
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
