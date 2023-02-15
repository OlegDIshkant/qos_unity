using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Просит расчитывать положения карт игрока, когда тот начинает выбирать себе карты среди карт другого игрока.
    /// </summary>
    public class WhenHideBeginCalcTFormsStartegy : IBeginCalcTFormsStartegy<CardId>
    {
        private readonly PlayerId _playerId;
        private readonly ICardsActionsProvider _cardsActionsProvider;

        public bool ToSkipRecal => false;


        public WhenHideBeginCalcTFormsStartegy(PlayerId playerId, ICardsActionsProvider cardsActionsProvider)
        {
            _playerId = playerId;
            _cardsActionsProvider = cardsActionsProvider;
        }


        public bool IsBeginCalcTFormsEvent(out IEnumerable<CardId> cardIds)
        {
            cardIds = CardsStartedGoingToHideMode();
            if (cardIds.Any()) return true;

            cardIds = CardsStartedTransferingToThisPlayer();
            return cardIds.Any(); 
        }


        private IEnumerable<CardId> CardsStartedGoingToHideMode()
        {
            foreach (var item in _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId))
            {
                if (item.Value.IsGoingToHideMode(out var normTime, out var _) && normTime == NormValue.Min)
                {
                    yield return item.Key;
                }
            }
        }


        private IEnumerable<CardId> CardsStartedTransferingToThisPlayer()
        {
            foreach (var item in _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId))
            {
                if (item.Value.IsGoingFromPlayerToPlayer(out var normTime, out var _, out var taker) && 
                    taker.Equals(_playerId) &&
                    normTime == NormValue.Min)
                {
                    yield return item.Key;
                }
            }
        }


        public bool IsStopCalcTFormsEvent(out IEnumerable<CardId> cardIds)
        {
            cardIds = CardsStartedGoingOutHideMode();
            return cardIds.Any();
        }


        private IEnumerable<CardId> CardsStartedGoingOutHideMode()
        {
            foreach (var item in _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId))
            {
                if (item.Value.IsGoingOutHideMode(out var normTime, out var _) && normTime == NormValue.Min)
                {
                    yield return item.Key;
                }
            }
        }
    }
}