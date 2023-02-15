using Qos.Domain.Entities;
using System.Collections.Generic;
using System.Linq;


namespace Qos.GameLogic.GameWorld.Activities
{
    /// <summary>
    /// Разрешает "сброс" карт, если каждая группа:
    /// <para>  1. Состоит ровно из двух карт.  </para>
    /// <para>  2. Эти карты образуют пару друг с другом.    </para>
    /// </summary>
    internal class StandartDiscardPolicy : ICardsDiscardPolicy
    {
        private readonly ICardMatchingStrategy _matchingStrategy;

        public IReadOnlyDictionary<CardId, CardModel> _cardModels;


        public StandartDiscardPolicy(IReadOnlyDictionary<CardId, CardModel> cardModels, ICardMatchingStrategy matchingStrategy)
        {
            _cardModels = cardModels;
            _matchingStrategy = matchingStrategy;
        }


        public bool MayDiscard(CardGroups cardGroups)
        {
            foreach(var group in cardGroups)
            {
                if (!IsDiscardable(group))
                {
                    return false;
                }
            }
            return true;
        }


        private bool IsDiscardable(HashSet<CardId> group)
        {
            if (group.Count != 2)
            {
                return false;
            }

            var cardA = _cardModels[group.First()];
            var cardB = _cardModels[group.Last()];

            return _matchingStrategy.CanBeMatched(new List<CardModel>() { cardA, cardB });
        }
    }
}
