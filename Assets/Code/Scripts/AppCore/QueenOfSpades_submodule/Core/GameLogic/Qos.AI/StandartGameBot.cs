using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using static Qos.Domain.Events.PlayerStartIdleEvent;


namespace Qos.AI
{
    public class StandartGameBot : AbstractGameBot
    {
        private CardsHelper _cardsHelper = new CardsHelper();

        private Dictionary<Type, Action<IEvent>> _eventHandlers;

        public StandartGameBot(PlayerId playerId) : base(playerId)
        {
            _eventHandlers = new Dictionary<Type, Action<IEvent>>()
            {
                { typeof(PlayersExpectedEvent), e => HandlePlayerExpected((PlayersExpectedEvent)e) },
                { typeof(PlayerStartIdleEvent), e => HandlePlayerStartIdle((PlayerStartIdleEvent)e) },
                { typeof(PlayerChoosingCardsForDiscardEvent), e => HandlePlayerChoosingCardsForDiscard((PlayerChoosingCardsForDiscardEvent)e) },
                { typeof(PlayerChoosingCardsForTrasferEvent), e => HandlePlayerChoosingCardsForTransfer((PlayerChoosingCardsForTrasferEvent)e) },
                { typeof(PlayerHasCardsEvent), e => HandlePlayerHasCards((PlayerHasCardsEvent)e) },
                { typeof(CardCreatedEvent), e => HandleCardCreated((CardCreatedEvent)e) },
                { typeof(PlayerLostCardsEvent), e => HandlePlayerLostCards((PlayerLostCardsEvent)e) },
            };
        }


        public override void Iterate(IEnumerable<IEvent> newEvents)
        {
            foreach (var @event in newEvents)
            {
                if (_eventHandlers.TryGetValue(@event.GetType(), out var handler))
                {
                    handler(@event);
                }
            }
        }


        private void HandlePlayerExpected(PlayersExpectedEvent @event)
        {
            var confirmer = @event.Confirmers.Single().Value;
            new PlayerConfirmationHandler()
                .Start(ToConfirmPlayer: () => confirmer.Execute());
        }


        private void HandlePlayerStartIdle(PlayerStartIdleEvent @event)
        {
            if (MayGoToDiscMode())
            {
                if (ShouldGoToDiscMode())
                {
                    GoToDiscMode();
                }
                else
                {
                    DenyGoingToDiscMode();
                }
            }

            bool MayGoToDiscMode() => @event.PlayerId.Equals(PlayerId) && @event.Interaction != null;
            bool ShouldGoToDiscMode() => _cardsHelper.HasCardsToDiscard();
            void GoToDiscMode() => new DiscardModeHandler().OnCanRequestDiscardMode(@event.Interaction);
            void DenyGoingToDiscMode() => @event.Interaction.Execute(new InteractionArg(InteractionArg.Message.WONT_REQUEST_DISCARD_MODE));
        }


        private void HandlePlayerChoosingCardsForDiscard(PlayerChoosingCardsForDiscardEvent @event)
        {
            if (@event.PlayerId.Equals(PlayerId))
            {
                var args = _cardsHelper.HasCardsToDiscard(out var cardsToDiscard) ?
                    new PlayerChoosingCardsForDiscardEvent.InteractionArg() { selectedCardGroups = cardsToDiscard } :
                    new PlayerChoosingCardsForDiscardEvent.InteractionArg() { cancelDiscarding = true };

                if (@event.Interaction.IsAlive)
                {
                    @event.Interaction.Execute(args);
                }
            }
        }


        private void HandlePlayerChoosingCardsForTransfer(PlayerChoosingCardsForTrasferEvent @event)
        {
            if (@event.CardTakerId.Equals(PlayerId))
            {
                var selectedCards = _cardsHelper.FindCardToTransfer(@event.PossibleCards).WrapInNewList();
                var args = new PlayerChoosingCardsForTrasferEvent.InteractionArg(selectedCards);

                if (@event.Interaction.IsAlive)
                {
                    @event.Interaction.Execute(args);
                }
            }
        }


        private void HandlePlayerHasCards(PlayerHasCardsEvent @event)
        {
            if (@event.PlayerId.Equals(PlayerId))
            {
                _cardsHelper.SetPlayerCards(@event.CardIds);
            }
        }


        private void HandleCardCreated(CardCreatedEvent @event)
        {
            _cardsHelper.RememberModel(@event.CardId, @event.CardModel);
        }


        private void HandlePlayerLostCards(PlayerLostCardsEvent @event)
        {
            if (@event.PlayerId.Equals(PlayerId))
            {
                _cardsHelper.RemoveSomePlayerCards(@event.CardIds);
            }
        }

    }
}
