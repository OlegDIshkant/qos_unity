using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Qos.Domain.Events.PlayerStartIdleEvent;
using static Qos.GameLogic.GameWorld.Helper;
using Feedback = Qos.Domain.Events.PlayerChoosingCardsForDiscardEvent.InteractionArg;
using FeedbackValidator = Qos.Domain.Events.PlayerChoosingCardsForDiscardEvent.InteractionArgValidator;
using Message = Qos.Domain.Events.PlayerStartIdleEvent.InteractionArg.Message;


namespace Qos.GameLogic.GameWorld.Activities
{
    internal class PlayersCards : Dictionary<PlayerId, HashSet<CardId>> { };

    /// <summary>
    /// Процесс, во время которого игрок может попробовать "сбросить" свои карты. 
    /// </summary>
    internal class DiscardingActivity : AbstractPlayerActivity
    {
        private readonly PlayerId _player;
        private readonly HashSet<CardId> _playersCards;
        private readonly TimeContext _timeContext;
        private readonly ICardsDiscardPolicy _discardPolicy;

        private bool PlayerLostAllCards { get; set; } = false;
        public IEnumerable<CardId> PlayerCards => new List<CardId>(_playersCards);

        public DiscardingActivity(
            PlayerId player, 
            TimeContext timeContext, 
            ICardsDiscardPolicy discardPolicy, 
            HashSet<CardId> playersCardsToModify)
        {
            _player = player;
            _playersCards = playersCardsToModify;
            _timeContext = timeContext;
            _discardPolicy = discardPolicy;
        }


        protected override bool AllowToStartExecution() => PlayerHasCards();


        private bool PlayerHasCards() => _playersCards.Any();


        protected override IEnumerator Executing()
        {
            Message? recievedMessage = null;
            var interaction = new BaseInteraction<InteractionArg>(result => recievedMessage = result.Msg);
            yield return new PlayerStartIdleEvent(_player, interaction);

            while (recievedMessage == null && !IsCancelled)
            {
                yield return null;
            }

            if (IsCancelled)
            {
                interaction.Kill();
            }
            else if (recievedMessage.Value == Message.WONT_REQUEST_DISCARD_MODE)
            {
                Cancel();
            }
            else if (recievedMessage.Value == Message.REQUEST_DISCARD_MODE)
            {
                yield return DiscardMode();
            }
            else
            {
                throw new Exception($"Unexpected value for message: {recievedMessage.Value}");
            }


            if (!PlayerLostAllCards)
            {
                yield return new PlayerStartIdleEvent(_player, interaction: null);
            }
        }


        private IEnumerator DiscardMode()
        {
            yield return RaiseContiniousEvent(new PlayerGoingToDiscardModeEvent(_player), 1000, _timeContext);

            while (!(IsCancelled || PlayerLostAllCards))
            {
                yield return DiscardModeIteration();
            }

            if (!PlayerLostAllCards)
            {
                yield return RaiseContiniousEvent(new PlayerGoingOutDiscardModeEvent(_player), 1000, _timeContext);
            }
        }


        private IEnumerator DiscardModeIteration()
        {
            Feedback feedback = null;
            bool gotFeedback = false;
            var interaction = new BaseInteraction<Feedback>(arg => { feedback = arg; gotFeedback = true; });

            yield return new PlayerChoosingCardsForDiscardEvent(_player, interaction);

            while (!(gotFeedback || IsCancelled))
            {
                yield return null;
            }

            if (IsCancelled)
            {
                interaction.Kill();
            }
            else if (!FeedbackValidator.IsValid(feedback, allowedCards: PlayerCards))
            {
                throw new Exception();
            }
            else if (feedback.cancelDiscarding)
            {
                Cancel();
            }
            else
            {
                yield return OnCardsSelected(feedback.selectedCardGroups);
            }
        }


        private IEnumerator OnCardsSelected(CardGroups selectedCardGroups)
        {
            if (_discardPolicy.MayDiscard(selectedCardGroups))
            {
                yield return DiscardingCards(AllCardsIn(selectedCardGroups));
            }
            else
            {
                yield return NotDiscardingCards(AllCardsIn(selectedCardGroups));
            }
        }


        private List<CardId> AllCardsIn(CardGroups groups) =>
            groups.SelectMany(group => group).Distinct().ToList();


        private IEnumerator DiscardingCards(List<CardId> discardingCardIds)
        {
            yield return RaiseContiniousEvent(new PlayerDiscardingCardsEvent(_player, discardingCardIds), 1_000, _timeContext);

            _playersCards.RemoveWhere(cardId => discardingCardIds.Contains(cardId));

            yield return new PlayerLostCardsEvent(_player, discardingCardIds);

            if (!_playersCards.Any())
            {
                PlayerLostAllCards = true;
            }
        }


        private IEnumerator NotDiscardingCards(List<CardId> cardIds)
        {
            yield return RaiseContiniousEvent(new PlayerNotDiscardingCardsEvent(_player, cardIds), 100, _timeContext);
            yield return new PlayerHasCardsEvent(_player, PlayerCards);
        }
    }
}
