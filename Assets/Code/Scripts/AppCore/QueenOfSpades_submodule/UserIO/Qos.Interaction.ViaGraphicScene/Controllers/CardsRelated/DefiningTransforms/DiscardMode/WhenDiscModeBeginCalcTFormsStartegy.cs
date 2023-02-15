using System.Collections.Generic;
using System.Linq;
using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Стратегия, определяющая для каких карт считать положения в пространстве, когда игрок (которому принадлежат карты)
    /// выбирает среди своих карт те, что собирается сбросить.
    /// </summary>
    public class WhenDiscModeBeginCalcTFormsStartegy : IBeginCalcTFormsStartegy<CardId>
    {

        private readonly PlayerId _playerId;
        private readonly ICardsActionsProvider _cardsActionsProvider;

        public bool ToSkipRecal => false;


        public WhenDiscModeBeginCalcTFormsStartegy(PlayerId playerId, ICardsActionsProvider cardsActionsProvider)
        {
            _playerId = playerId;
            _cardsActionsProvider = cardsActionsProvider;
        }


        public bool IsBeginCalcTFormsEvent(out IEnumerable<CardId> cardIds)
        {
            cardIds = CardsStartedGoingToDiscMode();
            return cardIds.Any();
        }


        private IEnumerable<CardId> CardsStartedGoingToDiscMode() =>
            _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId)
            .Where(i => i.Value.IsGoingToDiscMode(out var normTime, out var _) && normTime == NormValue.Min)
            .Select(i => i.Key);


        public bool IsStopCalcTFormsEvent(out IEnumerable<CardId> cardIds)
        {
            cardIds = CardsFinishedDiscarding();
            if (cardIds.Any())
            {
                return true;
            }

            cardIds = CardsStartedExitingDiscMode();
            return cardIds.Any();
        }



        private IEnumerable<CardId> CardsFinishedDiscarding() =>
            _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId)
            .Where(i => i.Value.IsGoingFromPlayerToHeapNow(out var normTime, out var _) && normTime == NormValue.Max)
            .Select(i => i.Key);



        private IEnumerable<CardId> CardsStartedExitingDiscMode() =>
            _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId)
            .Where(i => i.Value.IsGoingOutDiscMode(out var normTime, out var _) && normTime == NormValue.Min)
            .Select(i => i.Key);

    }
}