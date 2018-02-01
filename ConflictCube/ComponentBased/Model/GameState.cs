using ConflictCube.ComponentBased.Components;
using ConflictCube.ComponentBased.Controller;
using ConflictCube.ComponentBased.Model.Components.ParticleSystem;
using ConflictCube.ComponentBased.View;
using ConflictCube.ResxFiles;
using OpenTK;
using System;
using Zenseless.OpenGL;

namespace ConflictCube.ComponentBased
{
    public class GameState : IBuildScene, IBuildMenu
    {
        public Scene ActiveScene;

        private int WindowWidth;
        private int WindowHeight;
        
        public GameState(int windowWidth, int windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            BuildMenu();
        }

        public void BuildParticleSystemTest()
        {
            ClearGameObjectTree();

            ActiveScene = new Scene();

            GameObject root = new GameObject("root", new Transform(0f, 0f, 1f, 1f), null);

            GameObject test = new GameObject("test", new Transform(0f, 0f, .5f, .5f), root);
            //test.AddComponent(new Material(System.Drawing.Color.White));



            GameObject particleSystemObject = new GameObject("Shader", new Transform(0, 0, 1f, 1f), root);
            Material particleMaterial = new Material(System.Drawing.Color.Pink, TextureLoader.FromBitmap(ParticleSystemResources.Particle_Sprite_Smoke_1), new Zenseless.Geometry.Box2D(0, 0, 1, 1));
            ParticleSystem system = new ParticleSystem(100, .3f, particleMaterial, new Vector2(.15f), 5f, new Func<float, float>((r) => { return r + 1; }), new Func<float, float>((r) => { return 0.5f * r + 0.1f; }), Vector2.UnitY, 30);
            particleSystemObject.AddComponent(system);


            ActiveScene.RootGameObject = root;
            ActiveScene.Cameras = new System.Collections.Generic.List<Camera>() { new Camera(new Transform(0f, 0f, 1, 1), root, WindowWidth, WindowHeight, new Transform(), false) };
        
        }

        public void BuildShaderTest()
        {
            ClearGameObjectTree();

            ActiveScene = new Scene();

            GameObject root = new GameObject("root", new Transform(0f, 0f, 1f, 1f), null);

            GameObject shaderObject = new GameObject("Shader", new Transform(0, 0, .5f, .5f), root);
            //root.AddComponent(new Material(System.Drawing.Color.White));
            shaderObject.AddComponent(new Material(System.Drawing.Color.White, ResxFiles.ShaderResources.FragmentTest));

            GameObject test = new GameObject("test", new Transform(0.7f, 0f, .2f, .5f), root);
            test.AddComponent(new Material(System.Drawing.Color.White, Tilesets.Instance().NewFloorSheet.Tex, Tilesets.Instance().NewFloorSheet.CalcSpriteTexCoords(42), ResxFiles.ShaderResources.Liquid));

            GameObject test2 = new GameObject("test", new Transform(1.1f, 0f, .2f, .5f), root);
            test2.AddComponent(new Material(System.Drawing.Color.White, Tilesets.Instance().NewFloorSheet.Tex, Tilesets.Instance().NewFloorSheet.CalcSpriteTexCoords(42), ResxFiles.ShaderResources.Liquid));

            Camera camera = new Camera(new Transform(0f, 0f, 1, 1), root, WindowWidth, WindowHeight, new Transform(), false);
                        
            ActiveScene.RootGameObject = root;
            ActiveScene.Cameras = new System.Collections.Generic.List<Camera> { camera };
        }
        
        public void BuildMenu()
        {
            ClearGameObjectTree();

            ActiveScene = SceneBuilder.BuildMenu(this, WindowWidth, WindowHeight, null);
        }

        public void BuildScene(string level)
        {
            ClearGameObjectTree();

            ActiveScene = SceneBuilder.BuildLevel(this, level, new Transform(), WindowWidth, WindowHeight, null);
        }

        public void UpdateGameState()
        {
            //Axes
            //GameView.DrawDebug(new Transform(0, 0, 1f, .002f), Color.Red);
            //GameView.DrawDebug(new Transform(0, 0, .001f, 1f), Color.Blue);
            
            ActiveScene.RootGameObject.UpdateAll();
            GameObject.DestroyGameObjects();
        }

        private void ClearGameObjectTree()
        {
            if(ActiveScene.RootGameObject != null)
                GameObject.Destroy(ActiveScene.RootGameObject);
        }
        
        public ViewModel GetViewModel()
        {
            return new ViewModel(this);
        }
    }
}
