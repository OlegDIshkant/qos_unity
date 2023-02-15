using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections.Generic;
using System.Linq;


namespace Qos.ClientServer
{
    public class EventCensor : IEventCensor
    {
        public IEnumerable<IEvent> CensorEvents(PlayerId observer, IEnumerable<IEvent> originalEvents)
        {
            if (originalEvents == null)
            {
                return null;
            }

            var censoredEvents = new List<IEvent>();
            foreach (var originalEvent in originalEvents)
            {
                censoredEvents.Add(CreateCensoredEvent(originalEvent, observer));
            }
            return censoredEvents;
        }


        private IEvent CreateCensoredEvent(IEvent originalEvent, PlayerId observer)
        {
            IEvent censoredEvent = originalEvent;

            if (originalEvent is PlayersExpectedEvent playersExpectedEvent)
            {
                var censoredConfirmers =
                    playersExpectedEvent.Confirmers
                    .Where(item => item.Key.Equals(observer))
                    .ToDictionary(item => item.Key, item => item.Value); // Игрок может подтверждать лиш сам себя

                censoredEvent = new PlayersExpectedEvent(
                        playersExpectedEvent.MatchId,
                        playersExpectedEvent.PlayerIds,
                        censoredConfirmers);
            }
            else if (originalEvent is PlayerStartIdleEvent playerStartIdleEvent)
            {
                if (playerStartIdleEvent.Interaction != null &&
                    !playerStartIdleEvent.PlayerId.Equals(observer))
                {
                    censoredEvent = new PlayerStartIdleEvent(playerStartIdleEvent.PlayerId, interaction: null); //нельзя решать за других игроков
                }
            }
            else if (originalEvent is PlayerChoosingCardsForDiscardEvent playerChoosingCardsForDiscardEvent)
            {
                if (!playerChoosingCardsForDiscardEvent.PlayerId.Equals(observer))
                {
                    censoredEvent = new PlayerChoosingCardsForDiscardEvent(playerChoosingCardsForDiscardEvent.PlayerId, interaction: null); //нельзя решать за других игроков
                }
            }

            return censoredEvent;
        }
    }
}
