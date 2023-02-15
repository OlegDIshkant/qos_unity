using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Назначет игрокам индексы в соответствии с очередностью их хода.
    /// </summary>
    public class CalcIndexInPlayersOrder : ICalcIndexStrategy<PlayerId>
    {
        private readonly static int DUMMY_VALUE = -1;

        private readonly Dictionary<PlayerId, int> _playerIndex;

        public CalcIndexInPlayersOrder(IEnumerable<PlayerId> allPlayers, PlayerId mainPlayer)
        {
            _playerIndex = CalcOrderedPlayersList(allPlayers, mainPlayer);
        }


        private Dictionary<PlayerId, int> CalcOrderedPlayersList(IEnumerable<PlayerId> allPlayers, PlayerId mainPlayer)
        {
            var playersCycle = allPlayers.IterateInCycle();
            playersCycle.MoveNext();

            while (!playersCycle.Current.Equals(mainPlayer))
            {
                playersCycle.MoveNext();
            }

            var result = new Dictionary<PlayerId, int>();
            int i = 0;

            do
            {
                playersCycle.MoveNext();
                result.Add(playersCycle.Current, i++);
            }
            while (!playersCycle.Current.Equals(mainPlayer));

            return result;
        }


        public void AddToDictionary(IEnumerable<PlayerId> keys, Dictionary<PlayerId, int> dictionary)
        {
            AddWithDummyIndicies(keys, dictionary);
            RecalcIndiciesInDictionary(dictionary);
        }


        public void RemoveFromDictionary(IEnumerable<PlayerId> keys, Dictionary<PlayerId, int> dictionary)
        {
            RemoveKeys(keys, dictionary);
            RecalcIndiciesInDictionary(dictionary);
        }


        private void AddWithDummyIndicies(IEnumerable<PlayerId> keys, Dictionary<PlayerId, int> dictionary)
        {
            foreach (var key in keys)
            {
                dictionary.Add(key, DUMMY_VALUE);
            }
        }


        private void RecalcIndiciesInDictionary(Dictionary<PlayerId, int> dictionary)
        {
            var result = dictionary.OrderBy(i => _playerIndex[i.Key]);
            int i = 0;
            foreach (var item in result)
            {
                dictionary[item.Key] = i++;
            }
        }


        private void RemoveKeys(IEnumerable<PlayerId> keys, Dictionary<PlayerId, int> dictionary)
        {
            foreach (var key in keys)
            {
                dictionary.Remove(key);
            }
        }
    }
}