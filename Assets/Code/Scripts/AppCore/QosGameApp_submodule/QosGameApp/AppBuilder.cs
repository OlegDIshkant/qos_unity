using CGA;
using CGA.MainMenu;
using Qos.Interaction.ViaGraphicScene;
using System;


namespace QosGameApp.Build
{
    /// <summary>
    /// This class is the main builder of the whole game application.
    /// It should initialize all the parts of the app and integrate them in the final <see cref="IApp"/> instance.
    /// </summary>
    public class AppBuilder
    {
        public IApp BuildApplication(AbstractMainMenu mainMenu, Func<IGraphicSceneFactory> CreateGraphicSceneFactory)
        {
            IAppCoreFactory factory = new QosGameFactory(CreateGraphicSceneFactory);
            var app = new GameApp(mainMenu, factory);
            return app;
        }


    }
}
