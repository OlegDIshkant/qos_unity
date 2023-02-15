using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using static Qos.Domain.Events.PlayerChoosingCardsForTrasferEvent;
using static Qos.GameLogic.GameWorld.Helper;


namespace Qos.GameLogic.GameWorld.Activities
{
    /// <summary>
    /// Процесс, во время которого один игрок берет определенное кол-во карт у другого игрока. 
    /// </summary>
    internal class TransferActivity : AbstractPlayerActivity
    {

        private readonly PlayerId _giverId;
        private readonly PlayerId _takerId;
        private readonly PlayersCards _playersCards;
        private readonly TimeContext _timeContext;
        
        private InteractionArg _feedback;


        public TransferActivity(
            TimeContext timeContext,
            PlayerId giverId,
            PlayerId takerId,
            ref PlayersCards playersCards)
        {
            _timeContext = timeContext;
            _giverId = giverId;
            _takerId = takerId;
            _playersCards = playersCards;
        }


        protected override bool AllowCancelling => false;


        protected override bool AllowToStartExecution()
        {
            return _playersCards[_giverId].Count() > 0;

        }

        protected override IEnumerator Executing()
        {
            yield return RaiseContiniousEvent(new PlayersGoingToTrasferModeEvent(_giverId, _takerId), 1000, _timeContext);
            yield return InTransferModeEvent();
            yield return RaiseContiniousEvent(new PlayersGoingOutTrasferModeEvent(_giverId, _takerId), 1000, _timeContext);
        }


        private IEnumerator InTransferModeEvent()
        {
            _feedback = null;
            var interaction = new BaseInteraction<InteractionArg>(OnFeedback);

            yield return new PlayerChoosingCardsForTrasferEvent(_giverId, _takerId, _playersCards[_giverId], interaction);

            yield return WaitForFeedback(OnTimeout: () => Trace.WriteLine("Время ожидания выбора карты истекло."));

            var transferCard = FigureOutCardForTransfer();
            _playersCards[_giverId].Remove(transferCard);
            _playersCards[_takerId].Add(transferCard);

            var transferCards = transferCard.WrapInNewList();
            yield return new CardsChosenForTrasferEvent(_giverId, _takerId, transferCards);
            yield return RaiseContiniousEvent(new TrasferingCardsEvent(_giverId, _takerId, transferCards), 1000, _timeContext);

            yield return new PlayerLostCardsEvent(_giverId, transferCards);
            yield return new PlayerHasCardsEvent(_takerId, _playersCards[_takerId]);
        }


        private void OnFeedback(InteractionArg arg)
        {
            _feedback = arg;
        }


        private IEnumerator WaitForFeedback(Action OnTimeout)
        {
            var timeout = false;
            var timeoutWaiting = RoutineHelper.ExecuteWithAfteraction(
                _timeContext.WaitTime(60 * 1000), 
                () => timeout = true); 

            yield return RoutineHelper.WaitingWhile(() => _feedback == null && !timeout);

            if (_feedback == null)
            {
                OnTimeout?.Invoke();
            }

        }


        private CardId FigureOutCardForTransfer()
        {
            var card = _feedback.SelectedCards.First();
            if (!GiverHasSelectedCard())
            {
                var otherCard = _playersCards[_giverId].First();
                Logger.Error($"У игрока '{_giverId}' нету карты '{card}', но его просят её передать. Будет выбрана друая карта '{otherCard}'.");
                card = otherCard;
            }
            return card;


            bool GiverHasSelectedCard() => _playersCards[_giverId].Contains(card);
        }

    }
}