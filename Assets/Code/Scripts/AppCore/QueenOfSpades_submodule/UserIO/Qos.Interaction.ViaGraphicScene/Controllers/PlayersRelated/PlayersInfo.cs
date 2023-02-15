using CommonTools;
using Qos.Domain.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    public class PlayersInfo
    {
        public readonly PlayerId mainPLayerId;
        public readonly ReadOnlyCollection<PlayerId> otherPLayerIds;
        /// <summary>
        /// Все игроки в порядке совершения ходов.
        /// </summary>
        public readonly ReadOnlyCollection<PlayerId> allPLayerIds;

        public int AllPlayersAmount => OtherPlayersAmount + 1;
        public int OtherPlayersAmount => otherPLayerIds.Count;

        public PlayersInfo(PlayerId mainPLayerId, List<PlayerId> otherPLayerIds)
        {
            this.mainPLayerId = mainPLayerId;
            this.otherPLayerIds = new ReadOnlyCollection<PlayerId>(otherPLayerIds);

            var allPlayers = mainPLayerId.WrapInNewList();
            allPlayers.AddRange(otherPLayerIds);
            allPLayerIds = new ReadOnlyCollection<PlayerId>(allPlayers);
        }
    }
}
