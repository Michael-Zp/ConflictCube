using ConflictCube.ComponentBased.Model.Components.Colliders;
using ConflictCube.ComponentBased.Model.Components.Objects.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using Zenseless.Geometry;
using Zenseless.HLGL;

namespace ConflictCube.ComponentBased.Components.Objects.Tiles
{
    public class LevelTile : GameObject
    {
        

        public int Row { get; private set; }
        public int Column { get; private set; }
        public OnButtonChangeFloorEvent Event { private get; set; }

        private static Dictionary<int, Material> FloorTileMaterials = new Dictionary<int, Material>();
        private static Dictionary<GameObjectType, List<uint>> ReverseMaterialList = new Dictionary<GameObjectType, List<uint>>();
        private static GameObjectType[] TileIndexToObjectType = new GameObjectType[48];
        private static bool MaterialsAreInitialized = false;
        private Floor FloorOfTile;
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
            
            if(ReverseMaterialList.ContainsKey(type))
            {
                ReverseMaterialList.TryGetValue(type, out List<uint> values);
                values.Add(index);
            }
            else
            {
                ReverseMaterialList.Add(type, new List<uint>() { index });
            }
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

        public LevelTile(int row, int column, string name, Transform transform, int tileIndex, Floor floorOfTile, GameObject parent, OnButtonChangeFloorEvent changeEvent = null) : base(name, transform, parent)
        {
            if (!MaterialsAreInitialized)
            {
                InitalizeMaterials();
            }

            Row = row;
            Column = column;
            FloorOfTile = floorOfTile;
            TileIndex = tileIndex;
            Event = changeEvent;
            Type = GetGameObjectTypeForIndex(TileIndex);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            AddMaterialOnCreate();
            AddColliderOnCreate(FloorOfTile.CollisionGroup);
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
            else if (Type == GameObjectType.NotActiveButton)
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .9f, .9f), true, group, CollisionType.Finish));
            }
        }

        public override void OnCollision(Collider other)
        {
            if(Type != GameObjectType.NotActiveButton)
            {
                return;
            }

            Event?.StartEvent();

            ChangeFloorTile(GameObjectType.ActiveButton);
        }

        public void ChangeFloorTile(GameObjectType TypeToTransformTo)
        {
            if (TypeToTransformTo == Type)
            {
                return;
            }

            RemoveComponent<Material>();
            RemoveComponent<Collider>();

            ReverseMaterialList.TryGetValue(TypeToTransformTo, out List<uint> possibleTileIndies);

            Random random = new Random((int)Time.Time.CurrentTime);

            TileIndex = (int)possibleTileIndies[random.Next(0, possibleTileIndies.Count - 1)];
            
            Type = TypeToTransformTo;

            InitializeComponents();
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
    }
}
