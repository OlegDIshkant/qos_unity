using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.Controllers;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene
{
    /// <summary>
    /// Allows user to interact with the game via a grahical scene.
    /// </summary>
    public class GraphicSceneInteractor : DisposableClass, IInteractor
    {
        private readonly Contexts _contexts;
        private readonly Contexts.Control _contextsControl;

        private AbstractController _mainController;
        private MemorizingGraphicSceneFactory _graphicSceneFactory;
        private IEvent _currentEvent;

        public bool ExitRequested { get; private set; } = false;


        public GraphicSceneInteractor (IGraphicSceneFactory graphicSceneFactory, MatchInfo matchInfo)
        {
            _graphicSceneFactory = new MemorizingGraphicSceneFactory(graphicSceneFactory);
            _contexts = new Contexts(matchInfo.PlayersInfo, matchInfo.CardModels, out _contextsControl);
            _mainController = new MainController(_contexts, _graphicSceneFactory, () => _currentEvent, () => ExitRequested = true);
        }


        public void DepictEvents(ICollection<IEvent> events)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("");
            }

            _contextsControl.UpdateTimeContext();

            if (events.Count > 0)
            {
                foreach (var @event in events)
                {
                    DepictEvent(@event);
                }
            }
            else
            {
                DepictEvent(null); // Даже если не пришло событий, один апдейт случиться должен
            }
        }


        private void DepictEvent(IEvent @event)
        {
            Logger.Verbose($"\n\t Игровое событие:  '{@event}'\n");

            _currentEvent = @event; 
            ChangableData.MarkAllAsUnchanged();

            _mainController.Update();
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            _mainController = null;
            
            foreach (var obj in _graphicSceneFactory.CreatedObjects)
            {
                if (obj is IDisposable dispObj)
                {
                    dispObj.Dispose();
                }
            }

        }
    }






    public class MatchInfo
    {
        public PlayersInfo PlayersInfo { get; private set; }
        public IReadOnlyDictionary<CardId, CardModel> CardModels { get; private set; }


        public MatchInfo(PlayersInfo playersInfo, IReadOnlyDictionary<CardId, CardModel> cardModels)
        {
            PlayersInfo = playersInfo;
            CardModels = cardModels;
        }
    }


}
