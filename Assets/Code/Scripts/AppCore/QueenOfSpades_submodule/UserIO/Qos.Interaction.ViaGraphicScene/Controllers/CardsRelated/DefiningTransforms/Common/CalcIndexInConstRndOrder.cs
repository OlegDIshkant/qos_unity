using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Назначет картам индексы в соответствии с неким закрепленным рандомным порядком.
    /// </summary>
    public class CalcIndexInConstRndOrder : ICalcIndexStrategy<CardId>
    {
        private IComparer<CardId> _comparer;


        public CalcIndexInConstRndOrder(IReadOnlyCollection<CardId> allCardsInOrder)
        {
            _comparer = new DeterminedOrderComparerer<CardId>(allCardsInOrder);
        }


        public void AddToDictionary(IEnumerable<CardId> keys, Dictionary<CardId, int> dictionary)
        {
            RecalcCardIndicies(dictionary.Keys.Concat(keys), dictionary);
        }


        public void RemoveFromDictionary(IEnumerable<CardId> keys, Dictionary<CardId, int> dictionary)
        {
            foreach (var key in keys)
            {
                dictionary.Remove(key);
            }
            RecalcCardIndicies(dictionary.Keys, dictionary);
        }


        private void RecalcCardIndicies(IEnumerable<CardId> allCardIds, Dictionary<CardId, int> dictionary)
        {
            var cardOrder = allCardIds.OrderBy(c => c, _comparer).ToList();
            int i = 0;
            foreach (var card in cardOrder)
            {
                dictionary[card] = i++;
            }
        }
    }


}