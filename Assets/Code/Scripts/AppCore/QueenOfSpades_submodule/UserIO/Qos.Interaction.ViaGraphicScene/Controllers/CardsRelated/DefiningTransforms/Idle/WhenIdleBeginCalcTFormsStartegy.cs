using System.Collections.Generic;
using System.Linq;
using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CanonCardTransforms
{
    /// <summary>
    /// Стратегия, определяющая когда и для каких карт (принадлежащих игроку) считать положения в пространстве для режима "игрок ничем не занят".
    /// </summary>
    public class WhenIdleBeginCalcTFormsStartegy : IBeginCalcTFormsStartegy<CardId>
    {
        private readonly PlayerId _playerId;
        private readonly ICardsActionsProvider _cardsActionsProvider;

        public bool ToSkipRecal => false;

        public WhenIdleBeginCalcTFormsStartegy(PlayerId playerId, ICardsActionsProvider cardsActionsProvider)
        {
            _playerId = playerId;
            _cardsActionsProvider = cardsActionsProvider;
        }


        public bool IsBeginCalcTFormsEvent(out IEnumerable<CardId> cardIds)
        {
            cardIds = CardsStartedToBeRecieved();
            if (cardIds.Any()) 
                return true;

            cardIds = CardsStartedToLeaveDiscMode();
            if (cardIds.Any()) return true;

            cardIds = CardsStartedToLeaveTransferMode();
            if (cardIds.Any()) return true;

            cardIds = CardsStartedToLeaveHideMode();
            if (cardIds.Any()) return true;


            cardIds = null;
            return false;
        }


        private IEnumerable<CardId> CardsStartedToBeRecieved() =>
            _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId)
                .Where(i => i.Value.IsGoingFromDeckToPlayerNow(out var normTime, out var _) && normTime == NormValue.Min)
                .Select(i => i.Key)
                .ToList();


        private IEnumerable<CardId> CardsStartedToLeaveDiscMode()
        {
            return _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId)
                .Where(i => i.Value.IsGoingOutDiscMode(out var normTime, out var _) && normTime == NormValue.Min)
                .Select(i => i.Key)
                .ToList();
        }


        private IEnumerable<CardId> CardsStartedToLeaveTransferMode()
        {
            return _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId)
                .Where(i => i.Value.IsGoingOutTransferMode(out var normTime, out var _, out var _) && normTime == NormValue.Min)
                .Select(i => i.Key)
                .ToList();
        }


        private IEnumerable<CardId> CardsStartedToLeaveHideMode()
        {
            return _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId)
                .Where(i => i.Value.IsGoingOutHideMode(out var normTime, out var _) && normTime == NormValue.Min)
                .Select(i => i.Key)
                .ToList();
        }


        public bool IsStopCalcTFormsEvent(out IEnumerable<CardId> cardIds)
        {
            cardIds = CardsStopedIdling();
            return cardIds.Any();
        }


        private IEnumerable<CardId> CardsStopedIdling()
        {
            var allPrevActions = _cardsActionsProvider.PreviousActions.PlayerCardActions;

            if (allPrevActions?.ContainsKey(_playerId) ?? false)
            {
                var prevActions = allPrevActions[_playerId].Items;
                var changedActions = _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId);

                foreach (var item in changedActions)
                {
                    if (prevActions.TryGetValue(item.Key, out var prevAction))
                    {
                        if (prevAction.IsIdleNow(out var _) && !item.Value.IsIdleNow(out var _))
                        {
                            yield return item.Key;
                        }
                    }
                }
            }
        }
        


    }
}