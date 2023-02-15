using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections;
using static Qos.GameLogic.GameWorld.Helper;


namespace Qos.GameLogic.GameWorld.Stages
{
    internal class TerminalStage : AbstractStage
    {
        private readonly MatchId _matchId;
        private readonly TimeContext _timeContext;

        public TerminalStage(MatchId matchId, TimeContext timeContext)
        {
            _matchId = matchId;
            _timeContext = timeContext;
        }


        public override IEnumerator Complete()
        {
            yield return RaiseContiniousEvent(new MatchFinsihedEvent(_matchId), 2_000, _timeContext);
        }
    }
}