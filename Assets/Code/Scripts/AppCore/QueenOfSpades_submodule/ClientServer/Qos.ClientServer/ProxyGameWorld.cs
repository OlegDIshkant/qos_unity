using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.GameLogic.GameWorld;
using System.Collections.Generic;

namespace Qos.ClientServer
{
    /// <summary>
    /// Прокси игрового мира, задача которого ограничивать,
    /// что может видеть и делать с миром любой из пользователей.
    /// </summary>
    public class ProxyGameWorld
    {
        private IGameWorld _gameWorld;
        private Dictionary<PlayerId, IEventStream> _gameWorldObservers;
        private IEventCensor _eventCensor;

        public ProxyGameWorld(IGameWorld actualGameWorld, Dictionary<PlayerId, IEventStream> gameWorldObservers, IEventCensor eventCensor)
        {
            _gameWorld = actualGameWorld;
            _gameWorldObservers = gameWorldObservers;
            _eventCensor = eventCensor;
        }


        public void Iterate()
        {
            var events = _gameWorld.Iterate();
            if (events != null)
            {
                NotifyObserversAbout(events);
            }
        }


        private void NotifyObserversAbout(IEnumerable<IEvent> events)
        {
            foreach (var item in _gameWorldObservers)
            {
                var observer = item.Key;
                var observerStream = item.Value;

                var censoredEvents = _eventCensor.CensorEvents(observer, events);
                observerStream.PushEvents(censoredEvents);
            }
        }

    }
}
