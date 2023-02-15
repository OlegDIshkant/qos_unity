using Qos.Domain.Entities;
using System.Collections.Generic;


namespace Qos.GameLogic.GameWorld.Stages
{
    /// <summary>
    /// Временный тасовщик карт для тестирования работы.
    /// </summary>
    internal class TestCardsShaffler : ICardsShaffler
    {
        public IEnumerable<CardId> Shaffle(IEnumerable<CardId> cards)
        {
            var result = new List<CardId>();
            //Каждую третюю карту кладем в начало
            int counter = 0;
            foreach (var card in cards)
            {
                if (counter >= 2)
                {
                    counter = 0;
                    result.Add(card);
                }
                else
                {
                    counter++;
                    result.Insert(0, card);
                }
            }
            return result;
        }
    }
}
