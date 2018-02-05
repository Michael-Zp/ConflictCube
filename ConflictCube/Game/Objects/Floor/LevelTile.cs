using Engine.Components;
using Engine.Time;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using Zenseless.Geometry;
using Zenseless.HLGL;
using Zenseless.OpenGL;

namespace ConflictCube.Objects
{
    public class LevelTile : GameObject
    {
#pragma warning disable 0649

        [Import(typeof(ITime))]
        private ITime Time;

#pragma warning restore 0649

        public int Row { get; private set; }
        public int Column { get; private set; }
        public OnButtonChangeFloorEvent Event { private get; set; }

        private static Material MagmaParticle;
        private static Material IceParticle;
        private static Dictionary<int, Material> FloorTileMaterials = new Dictionary<int, Material>();
        private static Dictionary<string, List<uint>> ReverseMaterialList = new Dictionary<string, List<uint>>();
        private static string[] TileIndexToObjectType = new string[48];
        private static bool MaterialsAreInitialized = false;
        private Floor FloorOfTile;
        private int TileIndex = 0;

        private static void InitalizeMaterials()
        {
            SpriteSheet spriteSheet = Tilesets.Instance().FloorSheet;
            AddTileMaterial("Finish", spriteSheet, 1);
            AddTileMaterial("Floor", spriteSheet, 2);

            for (uint i = 3; i <= 41; i++)
            {
                AddTileMaterial("Wall", spriteSheet, i);
            }

            AddTileMaterial("None", spriteSheet, 42); //Not used but I dont want to change the spritesheet again...

            AddTileMaterial("OrangeFloor", spriteSheet, 43, ResxFiles.ShaderResources.Liquid);
            AddTileMaterial("BlueFloor", spriteSheet, 44, ResxFiles.ShaderResources.Liquid);
            AddTileMaterial("NotActiveButton", spriteSheet, 45);
            AddTileMaterial("ActiveButton", spriteSheet, 46);
            AddTileMaterial("BlueBlock", spriteSheet, 47);
            AddTileMaterial("OrangeBlock", spriteSheet, 48);


            IceParticle = new Material(Color.White, TextureLoader.FromBitmap(ResxFiles.ParticleSystemResources.IceParticle), new Box2D(Box2D.BOX01));
            MagmaParticle = new Material(Color.White, TextureLoader.FromBitmap(ResxFiles.ParticleSystemResources.MagmaParticle), new Box2D(Box2D.BOX01));

            MaterialsAreInitialized = true;
        }

        private static void AddTileMaterial(string type, SpriteSheet spriteSheet, uint index, string fragmentShader = null)
        {
            FloorTileMaterials.Add((int)index, new Material(Color.White, spriteSheet.Tex, new Box2D(spriteSheet.CalcSpriteTexCoords(index - 1)), fragmentShader));
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

        public static string GetstringForIndex(int index)
        {
            if(!MaterialsAreInitialized)
            {
                InitalizeMaterials();
            }

            if(index <= 0)
            {
                return "None";
            }

            return TileIndexToObjectType[index - 1];
        }

        public LevelTile(int row, int column, string name, Transform transform, int tileIndex, Floor floorOfTile, GameObject parent, OnButtonChangeFloorEvent changeEvent = null) : base(name, transform, parent)
        {
            Program.Container.ComposeParts(this);

            if (!MaterialsAreInitialized)
            {
                InitalizeMaterials();
            }

            Row = row;
            Column = column;
            FloorOfTile = floorOfTile;
            TileIndex = tileIndex;
            Event = changeEvent;
            Type = GetstringForIndex(TileIndex);
            
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            AddMaterialOnCreate();
            AddColliderOnCreate(FloorOfTile.CollisionGroup);
            AddOtherComponents();
        }

        private void AddOtherComponents()
        {
            if(Type.Equals("BlueBlock"))
            {
                ParticleSystem iceSystem = new ParticleSystem(50, .01f, IceParticle, new Vector2(.05f), .8f, null, null, Vector2.UnitY, 360)
                {
                    Enabled = false
                };

                AddComponent(iceSystem);
            }
            else if(Type.Equals("OrangeBlock"))
            {
                ParticleSystem magmaSystem = new ParticleSystem(50, .01f, MagmaParticle, new Vector2(.05f), .8f, null, null, Vector2.UnitY, 360)
                {
                    Enabled = false
                };

                AddComponent(magmaSystem);
            }
        }

        private void AddMaterialOnCreate()
        {
            FloorTileMaterials.TryGetValue(TileIndex, out Material material);

            //Only add a clone. Otherwise every change on one material will affect the others (like disable one -> disables all ^^)
            AddComponent(material?.Clone());
        }

        private void AddColliderOnCreate(CollisionGroup group)
        {
            if (Type.Equals("Wall"))
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), false, group, "Wall"));
            }
            else if (Type.Equals("OrangeBlock"))
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .85f, .85f), false, group, "OrangeBlock"));
            }
            else if (Type.Equals("BlueBlock"))
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .85f, .85f), false, group, "BlueBlock"));
            }
            else if (Type.Equals("OrangeFloor"))
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .7f, .7f), false, group, "OrangeFloor", "Orange"));
            }
            else if (Type.Equals("BlueFloor"))
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .7f, .7f), false, group, "BlueFloor", "Blue"));
            }
            else if (Type.Equals("Hole"))
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .6f, .6f), false, group, "Hole"));
            }
            else if (Type.Equals("Finish"))
            {
                AddComponent(new BoxCollider(new Transform(0, 0, 1, 1), true, group, "Finish"));
            }
            else if (Type.Equals("NotActiveButton"))
            {
                AddComponent(new BoxCollider(new Transform(0, 0, .9f, .9f), true, group, "Finish"));
            }
        }

        public override void OnCollision(Collider other)
        {
            if(!Type.Equals("NotActiveButton"))
            {
                return;
            }

            Event?.StartEvent();

            ChangeFloorTile("ActiveButton");
        }

        public void ChangeFloorTile(string TypeToTransformTo)
        {
            if (TypeToTransformTo == Type)
            {
                return;
            }

            RemoveComponent<Material>();
            RemoveComponent<Collider>();

            ReverseMaterialList.TryGetValue(TypeToTransformTo, out List<uint> possibleTileIndies);

            Random random = new Random((int)Time.CurrentTime);

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
                case "Floor":
                    ChangeFloorTile("Wall");
                    return true;

                case "Wall":
                    return false;

                case "Hole":
                    ChangeFloorTile("Floor");
                    return true;

                case "Finish":
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
