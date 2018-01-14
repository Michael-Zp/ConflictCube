using ConflictCube.ComponentBased.Model.Components.Colliders;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.Geometry;
using Zenseless.HLGL;

namespace ConflictCube.ComponentBased.Components.Objects.Tiles
{
    public class LevelTile : GameObject
    {
        public static Dictionary<int, Material> FloorTileMaterials = new Dictionary<int, Material>();

        public int Row { get; private set; }
        public int Column { get; private set; }


        private static GameObjectType[] TileIndexToObjectType = new GameObjectType[48];
        private static bool MaterialsAreInitialized = false;
        private Floor FloorOfTile;
        private int Health = 0;
        private int TileIndex = 0;

        private static void InitalizeMaterials()
        {
            SpriteSheet spriteSheet = Tilesets.Instance().NewFloorSheet;
            AddTileMaterial(GameObjectType.Finish, spriteSheet, 1);
            AddTileMaterial(GameObjectType.Floor, spriteSheet, 2);

            for (uint i = 3; i <= 41; i++)
            {
                AddTileMaterial(GameObjectType.Wall, spriteSheet, i);
            }

            AddTileMaterial(GameObjectType.None, spriteSheet, 42); //Not used but I dont want to change the spritesheet again...

            AddTileMaterial(GameObjectType.OrangeFloor, spriteSheet, 43);
            AddTileMaterial(GameObjectType.BlueFloor, spriteSheet, 44);
            AddTileMaterial(GameObjectType.NotActiveButton, spriteSheet, 45);
            AddTileMaterial(GameObjectType.ActiveButton, spriteSheet, 46);
            AddTileMaterial(GameObjectType.BlueBlock, spriteSheet, 47);
            AddTileMaterial(GameObjectType.OrangeBlock, spriteSheet, 48);


            MaterialsAreInitialized = true;
        }

        private static void AddTileMaterial(GameObjectType type, SpriteSheet spriteSheet, uint index)
        {
            FloorTileMaterials.Add((int)index, new Material(Color.White, spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(index - 1))));
            TileIndexToObjectType[index - 1] = type;
        }

        public static GameObjectType GetGameObjectTypeForIndex(int index)
        {
            if(!MaterialsAreInitialized)
            {
                InitalizeMaterials();
            }

            if(index <= 0)
            {
                return GameObjectType.None;
            }

            return TileIndexToObjectType[index - 1];
        }

        public LevelTile(int row, int column, string name, Transform transform, GameObject parent, int tileIndex, Floor floorOfTile) : base(name, transform, parent)
        {
            if (!MaterialsAreInitialized)
            {
                InitalizeMaterials();
            }

            Row = row;
            Column = column;
            FloorOfTile = floorOfTile;
            TileIndex = tileIndex;
            Type = GetGameObjectTypeForIndex(TileIndex);

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
            FloorTileMaterials.TryGetValue(TileIndex, out Material material);

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
                AddComponent(new BoxCollider(new Transform(0, 0, .85f, .85f), false, group, CollisionType.OrangeBlock));
            }
            else if (Type == GameObjectType.BlueBlock)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .85f, .85f), false, group, CollisionType.BlueBlock));
            }
            else if (Type == GameObjectType.OrangeFloor)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .7f, .7f), false, group, CollisionType.OrangeFloor, CollisionLayer.Orange));
            }
            else if (Type == GameObjectType.BlueFloor)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .7f, .7f), false, group, CollisionType.BlueFloor, CollisionLayer.Blue));
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
            LevelTile newGameObject = (LevelTile)base.Clone();

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
