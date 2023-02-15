using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Immutable;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated
{
    /// <summary>
    /// Контроллер, предоставляющий информацию о том, что в данный момент времени должно происходить с той или иной картой,
    /// а также, что должно было происходить с той или иной картой в предыдущий момент времени.
    /// </summary>
    public class CardActionsProvider : EventController, ICardsActionsProvider
    {
        private DefiningCardsActions _currentActions;
        private DefiningCardsActions _prevActions;

        private IEvent _currentEvent;
        private IEvent _prevEvent;

        public DictionaryData<CardId, CardAction> NonPlayerCardActions => _currentActions.NonPlayerCardActions;
        public ImmutableDictionary<PlayerId, DictionaryData<CardId, CardAction>> PlayerCardActions => _currentActions.PlayerCardActions;
        public DictionaryData<CardId, CardAction> AllCardActions => _currentActions.AllCardActions;

        public ICardActionsDefiner PreviousActions => _prevActions;


        public CardActionsProvider(Contexts contexts, Func<IEvent> GetCurrentEvent) : 
            base(contexts, GetCurrentEvent)
        {
            _currentActions = new DefiningCardsActions(contexts, this.GetCurrentEvent);
            _prevActions = new DefiningCardsActions(contexts, this.GetPrevEvent);
        }


        private IEvent GetCurrentEvent() => _currentEvent;


        private IEvent GetPrevEvent() => _prevEvent;


        public override void Update()
        {
            _prevEvent = _currentEvent;
            _currentEvent = CurrentEvent;

            _currentActions.Update();
            _prevActions.Update();
        }
    }
}