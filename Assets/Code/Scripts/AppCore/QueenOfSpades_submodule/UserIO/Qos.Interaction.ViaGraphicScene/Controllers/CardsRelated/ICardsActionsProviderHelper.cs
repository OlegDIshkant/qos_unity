using Qos.Domain.Entities;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated
{
    public static class CardsActionsProviderHelper
    {
        public static IEnumerable<KeyValuePair<CardId, CardAction>> GetAddedOrChangedPlayerActions(this ICardActionsDefiner cardsActionsProvider, PlayerId playerId)
        {
            if (cardsActionsProvider.PlayerCardActions == null) return Enumerable.Empty<KeyValuePair<CardId, CardAction>>();
            if (cardsActionsProvider.PlayerCardActions.TryGetValue(playerId, out var playerActions))
            {
                if (playerActions == null) return Enumerable.Empty<KeyValuePair<CardId, CardAction>>();
                return playerActions.AddedOrChanged;
            }
            return Enumerable.Empty<KeyValuePair<CardId, CardAction>>();
        }

        public static IDictionary<CardId, CardAction> GetPrevPlayerActions(this ICardsActionsProvider cardsActionsProvider, PlayerId playerId)
        {
            if (cardsActionsProvider.PreviousActions?.PlayerCardActions == null) return null;
            if (cardsActionsProvider.PreviousActions.PlayerCardActions.TryGetValue(playerId, out var playerActions))
            {
                if (playerActions == null) return null;
                return playerActions.Items;
            }
            return null;
        }

    }
}