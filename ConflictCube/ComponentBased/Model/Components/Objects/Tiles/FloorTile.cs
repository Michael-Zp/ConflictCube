using ConflictCube.ComponentBased.Model.Components.Colliders;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.Geometry;
using Zenseless.HLGL;

namespace ConflictCube.ComponentBased.Components.Objects.Tiles
{
    public class FloorTile : GameObject
    {
        public static Dictionary<GameObjectType, Material> FloorTileMaterials = new Dictionary<GameObjectType, Material>();

        public int Row { get; private set; }
        public int Column { get; private set; }

        private static bool MaterialsAreInitialized = false;
        private Floor FloorOfTile;
        private int Health = 0;

        private static void InitalizeMaterials()
        {
            SpriteSheet spriteSheet = Tilesets.Instance().FloorSheetIceFire;
            FloorTileMaterials.Add(GameObjectType.Finish, new Material(spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(0)), Color.White));
            FloorTileMaterials.Add(GameObjectType.Floor,  new Material(spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(1)), Color.White));
            FloorTileMaterials.Add(GameObjectType.Hole,   new Material(spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(2)), Color.White));
            FloorTileMaterials.Add(GameObjectType.Wall,   new Material(spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(3)), Color.White));
            FloorTileMaterials.Add(GameObjectType.OrangeBlock, new Material(spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(4)), Color.White));
            FloorTileMaterials.Add(GameObjectType.BlueBlock, new Material(spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(5)), Color.White));
            FloorTileMaterials.Add(GameObjectType.OrangeFloor, new Material(spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(6)), Color.White));
            FloorTileMaterials.Add(GameObjectType.BlueFloor, new Material(spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(7)), Color.White));
        }

        public FloorTile(int row, int column, string name, Transform transform, GameObject parent, GameObjectType type, Floor floorOfTile) : base(name, transform, parent, type)
        {
            if (!MaterialsAreInitialized)
            {
                InitalizeMaterials();
                MaterialsAreInitialized = true;
            }

            Row = row;
            Column = column;
            FloorOfTile = floorOfTile;

            InitializeComponentsAndHealth();
        }

        private void InitializeComponentsAndHealth()
        {
            AddMaterialOnCreate();
            AddColliderOnCreate(FloorOfTile.CollisionGroup);

            if (Type == GameObjectType.Wall)
            {
                Health = 3;
            }
        }

        private void AddMaterialOnCreate()
        {
            FloorTileMaterials.TryGetValue(Type, out Material material);

            AddComponent(material);
        }

        private void AddColliderOnCreate(CollisionGroup group)
        {
            if (Type == GameObjectType.Wall)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, group, CollisionType.Wall));
            }
            else if (Type == GameObjectType.OrangeBlock)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, group, CollisionType.OrangeBlock));
            }
            else if (Type == GameObjectType.BlueBlock)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, group, CollisionType.BlueBlock));
            }
            else if (Type == GameObjectType.OrangeFloor)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, group, CollisionType.OrangeFloor, CollisionLayer.Orange));
            }
            else if (Type == GameObjectType.BlueFloor)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, group, CollisionType.BlueFloor, CollisionLayer.Blue));
            }
            else if (Type == GameObjectType.Hole)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .6f, .6f), false, group, CollisionType.Hole));
            }
            else if (Type == GameObjectType.Finish)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), true, group, CollisionType.Finish));
            }
        }

        public void ChangeFloorTile(GameObjectType TypeToTransformTo)
        {
            if (TypeToTransformTo == Type)
            {
                return;
            }

            RemoveComponent<Material>();
            RemoveComponent<Collider>();

            Type = TypeToTransformTo;

            InitializeComponentsAndHealth();
        }

        /// <summary>
        /// Tries to set a cube on this floor tile. Change type from Floor->Wall or Hole->Floor.
        /// Returns true if the cube was used. False if the cube was not used (Trying to set it onto a wall or finish tile)
        /// </summary>
        /// <returns></returns>
        public bool PutCubeOnFloorTile()
        {
            switch(Type)
            {
                case GameObjectType.Floor:
                    ChangeFloorTile(GameObjectType.Wall);
                    return true;

                case GameObjectType.Wall:
                    return false;

                case GameObjectType.Hole:
                    ChangeFloorTile(GameObjectType.Floor);
                    return true;

                case GameObjectType.Finish:
                    return false;
            }

            return false;
        }

        public override GameObject Clone()
        {
            FloorTile newGameObject = (FloorTile)base.Clone();

            newGameObject.Row = Row;
            newGameObject.Column = Column;

            return newGameObject;
        }

        public void HitFloorTileWithSledgeHammer()
        {
            if (Type == GameObjectType.Wall)
            {
                Health--;
                if (Health <= 0)
                {
                    ChangeFloorTile(GameObjectType.Floor);
                }
            }
        }
    }
}
