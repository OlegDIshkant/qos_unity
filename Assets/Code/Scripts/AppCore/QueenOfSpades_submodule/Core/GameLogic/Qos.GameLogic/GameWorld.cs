using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.GameLogic.GameWorld.Activities;
using Qos.GameLogic.GameWorld.Stages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Qos.GameLogic.GameWorld.Helper;


namespace Qos.GameLogic.GameWorld
{
    public class GameWorld : BaseGameWorld
    {
        private readonly MatchId _matchId = new MatchId();
        private readonly IEnumerable<PlayerId> _playerIds;
        private readonly PlayersCards _playersCards = new PlayersCards();
        private readonly TimeContext _timeContext;
        private readonly Action _UpdateTimeContext;
        private readonly ICardModelsProvider _cardModelsProvider;

        private IEnumerator<IEnumerable<IEvent>> _mainRoutine;



        /// <param name="playerIds"> Игроки, которые будут учавствовать в матче в порядке совершения хода. </param>
        public GameWorld(IEnumerable<PlayerId> playerIds, ICardModelsProvider cardModelsProvider)
        {
            _timeContext = new TimeContext(out _UpdateTimeContext);
            _playerIds = playerIds;
            _cardModelsProvider = cardModelsProvider;
            _mainRoutine = GameEventsIterator(MainRoutine());

        }


        public override IEnumerable<IEvent> Iterate()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("");
            }

            _UpdateTimeContext();
            return _mainRoutine.MoveNext() ? _mainRoutine.Current : null;
        }


        private IEnumerator MainRoutine()
        {
            yield return new InitialStage(_playerIds, _matchId, _timeContext).Complete();
            yield return new CardsEnterStage(_cardModelsProvider, _playerIds, _timeContext, _playersCards).Complete();
            yield return new MatchStartedStage(_matchId, _playerIds, _timeContext).Complete();
            var discardPolicy = DefineDiscardPolicy(_cardModelsProvider.CardModels);
            yield return new OnlyDiscardingStage(_timeContext, _playersCards, _playerIds, discardPolicy).Complete();
            yield return new MakeTurnsStage(_timeContext, LeftPlayers(), _playersCards, discardPolicy).Complete();
            yield return new TerminalStage(_matchId, _timeContext).Complete();
        }


        private ICardsDiscardPolicy DefineDiscardPolicy(IReadOnlyDictionary<CardId, CardModel> cardModels)
        {
            return new StandartDiscardPolicy(cardModels, new StandartPlayingCardMatchingStrategy());
        }


        private HashSet<PlayerId> LeftPlayers()
        {
            return new HashSet<PlayerId>(_playerIds.Where(p => _playersCards[p].Any()));
        }
    }
}
