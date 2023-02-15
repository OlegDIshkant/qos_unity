using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Qos.Interaction.ViaGraphicScene
{

    /// <summary>
    /// <see cref="IGraphicSceneFactory"/>, которая запоминает все объекты, которые создает;
    /// </summary>
    public class MemorizingGraphicSceneFactory : IGraphicSceneFactory
    {
        private readonly IGraphicSceneFactory _decoratedFactory;
        private readonly List<object> _createdObjects;

        public IReadOnlyCollection<object> CreatedObjects { get; private set; }

        
        public MemorizingGraphicSceneFactory(IGraphicSceneFactory decoratedFactory)
        {
            _decoratedFactory = decoratedFactory;
            _createdObjects = new List<object>();
            CreatedObjects = new ReadOnlyCollection<object>(_createdObjects);
        }


        public ICamera CreateCamera() => WithRemembering(_decoratedFactory.CreateCamera());
        public ICard CreateCard(CardModel cardModel) => WithRemembering(_decoratedFactory.CreateCard(cardModel));
        public ICardHeap CreateCardHeap() => WithRemembering(_decoratedFactory.CreateCardHeap());
        public ICursor CreateCursor() => WithRemembering(_decoratedFactory.CreateCursor());
        public IDeck CreateDeck() => WithRemembering(_decoratedFactory.CreateDeck());
        public IDiscModeUi CreateDiscModeUi() => WithRemembering(_decoratedFactory.CreateDiscModeUi());
        public IPlayer CreatePlayer(PlayerModel model) => WithRemembering(_decoratedFactory.CreatePlayer(model));
        public IPlayField CreatePlayField() => WithRemembering(_decoratedFactory.CreatePlayField());
        public IWinLooseAnnouncer CreateWinLooseAnnouncer() => WithRemembering(_decoratedFactory.CreateWinLooseAnnouncer());
        public IExitUi CreateExitUi() => WithRemembering(_decoratedFactory.CreateExitUi());


        private T WithRemembering<T>(T createdObject)
        {
            _createdObjects.Add(createdObject);
            return createdObject;
        }
    }
}