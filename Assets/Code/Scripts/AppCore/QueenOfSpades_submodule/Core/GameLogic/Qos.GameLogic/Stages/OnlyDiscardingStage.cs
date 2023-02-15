using System.Collections.Generic;
using CommonTools;
using System.Linq;
using Qos.Domain.Entities;
using static Qos.GameLogic.GameWorld.Stages.AbstractMakeTurnStage;
using Qos.GameLogic.GameWorld.Activities;


namespace Qos.GameLogic.GameWorld.Stages
{
    /// <summary>
    /// Стадия, когда игрокам позволено лишь сбрасывать свои карты, если те могут.
    /// </summary>
    internal class OnlyDiscardingStage : AbstractMakeTurnStage
    {
        private static readonly int TIMEOUT_MILLISEC = 6 * 10 * 1000;

        public OnlyDiscardingStage(
            TimeContext timeContext,
            PlayersCards playersCards,
            IEnumerable<PlayerId> playerIds,
            ICardsDiscardPolicy discardPolicy) :
            base(
                new OnlyDiscardingActivitiesStrategy(timeContext, playersCards, playerIds, discardPolicy),
                new StandartTimeoutStrategy(timeContext, TIMEOUT_MILLISEC))
        {
        }

    }


    internal class OnlyDiscardingActivitiesStrategy : IWhatActivitiesStrategy
    {
        private TimeContext _timeContext;
        private PlayersCards _playersCards;
        private IEnumerable<PlayerId> _playerIds;
        private ICardsDiscardPolicy _discardPolicy;


        public OnlyDiscardingActivitiesStrategy(
            TimeContext timeContext,
            PlayersCards playersCards,
            IEnumerable<PlayerId> playerIds,
            ICardsDiscardPolicy discardPolicy)
        {
            _timeContext = timeContext;
            _playersCards = playersCards;
            _playerIds = playerIds;
            _discardPolicy = discardPolicy;
        }


        public ICollection<AbstractPlayerActivity> DefinePlayerActivitiesForTurn()
        {
            return _playerIds.Select(
                playerId => DiscardingActivityFor(playerId))
            .ToList();
        }


        private AbstractPlayerActivity DiscardingActivityFor(PlayerId player) =>
            new DiscardingActivity(
                player,
                _timeContext,
                _discardPolicy,
                _playersCards[player]);


    }
}
