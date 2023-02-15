using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using static Qos.GameLogic.GameWorld.Helper;


namespace Qos.GameLogic.GameWorld.Stages
{
    internal class MatchStartedStage : AbstractStage
    {
        private MatchId _matchId;
        private IEnumerable<PlayerId> _playersIds;
        private TimeContext _timeContext;


        public MatchStartedStage(MatchId matchId, IEnumerable<PlayerId> playersIds, TimeContext timeContext)
        {
            _matchId = matchId;
            _playersIds = playersIds;
            _timeContext = timeContext;
        }


        public override IEnumerator Complete()
        {
            yield return RaiseContiniousEvent(new MatchStartedEvent(_matchId, _playersIds), 1000, _timeContext);
            foreach (var player in _playersIds)
            {
                yield return new PlayerStartIdleEvent(player);
            }
        }
    }
}
