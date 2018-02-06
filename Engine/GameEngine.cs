using Engine.Controler;
using Engine.Model;
using Engine.Scenes;
using Engine.Time;
using Engine.View;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Engine
{
    public class GameEngine
    {
        public readonly SceneManager SceneManager;

        private readonly MyWindow Window;
        private readonly GameView View;
        private readonly GameState State;
        private readonly GameControler GameControler;

        public static CompositionContainer Container;

#pragma warning disable 0649

        [Import(typeof(ITimeSetter))]
        private ITimeSetter TimeSetter;
        
#pragma warning restore 0649

        public GameEngine()
        {
            InitMEF();

            Window = new MyWindow(512, 512);
            View = new GameView(Window);
            State = new GameState(Window.Width, Window.Height);
            GameControler = new GameControler();
            SceneManager = new SceneManager(State);
        }

        private void InitMEF()
        {
            //MEF
            //An aggregate catalog that combines multiple catalogs  
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class  
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(GameEngine).Assembly));

            //Create the CompositionContainer with the parts in the catalog  
            Container = new CompositionContainer(catalog);

            //Fill the imports of this object  
            try
            {
                Container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public void RunGameLoop()
        {
            while (Window.WaitForNextFrame())
            {
                TimeSetter.CurrentTime = Window.GetTime();

                GameControler.UpdateInputs();
                State.UpdateGameState();
                View.Render(State.GetViewModel());
            }
        }
    }
}
