using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections.Generic;
using static CommonTools.RoutineHelper;
using System.Collections;
using static Qos.GameLogic.GameWorld.Helper;


namespace Qos.GameLogic.GameWorld.Stages
{
    /// <summary>
    /// Самая ранняя стадия матча.
    /// </summary>
    internal class InitialStage : AbstractStage
    {
        private MatchId _matchId;
        private TimeContext _timeContext;

        private IEnumerable<PlayerId> _playerIds;
        private Dictionary<PlayerId, PlayerModel> _playerModels;

        private HashSet<PlayerId> _expectedPlayers = new HashSet<PlayerId>();
        private HashSet<PlayerId> _confirmedPlayers = new HashSet<PlayerId>();
        private Dictionary<PlayerId, BaseInteraction> _confirmers = new Dictionary<PlayerId, BaseInteraction>();
        private HashSet<PlayerId> _createdPlayers = new HashSet<PlayerId>();
        private List<IEnumerator<IEnumerable<IEvent>>> _createRoutines = new List<IEnumerator<IEnumerable<IEvent>>>();


        public InitialStage(IEnumerable<PlayerId> playerIds, MatchId matchId, TimeContext timeCotext)
        {
            _playerIds = playerIds;
            _matchId = matchId;
            _timeContext = timeCotext;
        }


        public override IEnumerator Complete()
        {
            yield return MatchInitiation();
            yield return PlayersExpecting();
            yield return PlayersCreating();
        }


        private IEnumerator MatchInitiation()
        {
            yield return RaiseContiniousEvent(new MatchInitedEvent(_matchId), 3000, _timeContext);
        }


        private IEnumerator PlayersExpecting()
        {
            InitPlayerSeats();
            yield return new PlayersExpectedEvent(_matchId, _expectedPlayers, _confirmers);
        }


        private void InitPlayerSeats()
        {
            _playerModels = new Dictionary<PlayerId, PlayerModel>();
            foreach (var id in _playerIds)
            {              
                _playerModels[id] = PlayerModel.GenerateNew($"Player {id}"); ;
                
                _expectedPlayers.Add(id);
                _confirmers.Add(id, new BaseInteraction(() => _confirmedPlayers.Add(id)));
            }
        }


        private IEnumerator PlayersCreating()
        {
            while (NotAllPlayersAreCreatedYet())
            {
                StartCreationOfConfirmedPlayers();
                yield return IterateCreationOfPlayers();
            }
        }


        private bool NotAllPlayersAreCreatedYet()
        {
            return _createdPlayers.Count < _expectedPlayers.Count
                || _confirmedPlayers.Count > 0;
        }


        private void StartCreationOfConfirmedPlayers()
        {
            if (_confirmedPlayers.Count > 0)
            {
                foreach (var playerId in _confirmedPlayers)
                {
                    var playerToCreate = playerId;
                    _createRoutines.Add(
                        GameEventsIterator(
                            ExecuteWithAfteraction(
                                RaiseContiniousEvent(new PlayerCreatedEvent(playerId, _playerModels[playerId]), 2000, _timeContext),
                                () => { _createdPlayers.Add(playerToCreate); })
                            )
                        );
                }
                _confirmedPlayers.Clear();
            }
        }


        private List<IEvent> IterateCreationOfPlayers()
        {
            var routinesToForget = new List<IEnumerator<IEnumerable<IEvent>>>();
            var events = new List<IEvent>();

            foreach (var routine in _createRoutines)
            {
                if (routine.MoveNext())
                {
                    events.AddRange(routine.Current);
                }
                else
                {
                    routinesToForget.Add(routine);
                }
            }

            foreach (var routine in routinesToForget)
            {
                _createRoutines.Remove(routine);
            }

            return events;
        }


    }



}
