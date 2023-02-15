using CommonTools;
using CommonTools.StatesManaging;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.GameLogic.GameWorld.Activities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Qos.GameLogic.GameWorld.Stages
{
    /// <summary>
    /// —тади€, когда игроки по очереди берут определеное кол-во карт из колоды соседа, а остальные в это врем€ могут сбрасывать карты.
    /// </summary>
    internal class MakeTurnsStage : AbstractStage
    {
        private enum States
        {
            TELLING_PLAYERS_WHAT_TO_DO,
            WAITING_FOR_CARD_TRANSITION_FINISHED,
            ALLOWING_EX_TRANSITION_MEMBERS_TO_DISCARD,
            DEFINING_NEXT_CARDS_TAKER,
            DEFINING_NEXT_CARDS_GIVER,
            FINISHING,
            FINISHED
        }

        private readonly static int SEC_FOR_FREE_TIME_AFTER_TAKING_CARDS = 10;

        private readonly StateChecker<States> _stateChecker = new StateChecker<States>(States.DEFINING_NEXT_CARDS_GIVER);
        private readonly TimeContext _timeContext;
        private readonly ICardsDiscardPolicy _discardPolicy;

        private PlayersCards _playerCards;
        private PlayerId? _nextTaker, _nextGiver;
        private PlayerId? _taker, _giver;
        private bool _toInitDiscards = true;
        private HashSet<PlayerId> _allPlayers;
        private HashSet<PlayerId> _playersLeft;
        private Helper.RoutinePool _routines = new Helper.RoutinePool();
        private Dictionary<PlayerId, PausableDiscardingActivity> _discardActivities = new Dictionary<PlayerId, PausableDiscardingActivity>();
        private TransferActivity _transferActivity;
        private IEnumerator<PlayerId> _playersCycle;


        public MakeTurnsStage(
            TimeContext timeContext, 
            HashSet<PlayerId> allPlayersWithCards,
            PlayersCards playerCards,
            ICardsDiscardPolicy discardPolicy) :
            base()
        {
            _timeContext = timeContext;
            _nextTaker = _nextGiver = _taker = _giver = null;
            _playerCards = playerCards;
            _discardPolicy = discardPolicy;

            _allPlayers = allPlayersWithCards;
            _playersLeft = new HashSet<PlayerId>(_allPlayers);
            _playersCycle = _allPlayers.IterateInCycle();
        }


        public override IEnumerator Complete()
        {
            yield return Helper.IteratateSimultaneously(StageLogic(), RoutinesRunning());
        }


        private IEnumerator StageLogic()
        {
            while (true)
            {
                switch (_stateChecker.CurrentState)
                {
                    case States.TELLING_PLAYERS_WHAT_TO_DO: yield return WhenTellingPlayersWhatToDo(); break;
                    case States.WAITING_FOR_CARD_TRANSITION_FINISHED: yield return WhenWaitingForTransitionFinished(); break;
                    case States.ALLOWING_EX_TRANSITION_MEMBERS_TO_DISCARD: yield return WhenAllowingTransitionMembersToDiscard(); break;
                    case States.DEFINING_NEXT_CARDS_GIVER: yield return WhenDefiningNextCardGiver(); break;
                    case States.DEFINING_NEXT_CARDS_TAKER: yield return WhenDefiningNextCardTaker(); break;
                    case States.FINISHING: yield return WhenFinishing(); yield break;
                    default: throw new Exception();
                }
            }
        }


        private IEnumerator RoutinesRunning()
        {
            while (NotFinished())
            {
                yield return _routines.Iterate();
            }

            bool NotFinished() => !_stateChecker.CurrentState.Equals(States.FINISHED);
        }


        private IEnumerator WhenTellingPlayersWhatToDo()
        {
            StartNewCardTransfer();
            StartDiscardsIfNeeded();
            _stateChecker.ChangeStateTo(States.WAITING_FOR_CARD_TRANSITION_FINISHED);
            yield return null;
        }


        private void StartNewCardTransfer()
        {
            Trace.Assert(_nextGiver != null && _nextTaker != null);
            _taker = _nextTaker;
            _giver = _nextGiver;
            _transferActivity = new TransferActivity(_timeContext, _giver.Value, _taker.Value, ref _playerCards);
            _routines.AddRoutine(_transferActivity.StartExecute());
        }


        private void StartDiscardsIfNeeded()
        {
            if (_toInitDiscards)
            {
                _toInitDiscards = false;

                var otherPlayers = _playersLeft.Where(p => !p.Equals(_taker) && !p.Equals(_giver));
                InitDiscardFor(otherPlayers);
            }
        }


        private IEnumerator WhenWaitingForTransitionFinished()
        {
            while (_transferActivity.IsExecuting)
            {
                yield return null;
            }
            yield return null;

            if (_playerCards[_giver.Value].Count <= 0)
            {
                yield return WinningMatch(_giver.Value);
            }

            if (_playersLeft.Count <= 0)
            {
                StartFinishing();
            }
            else
            {
                _stateChecker.ChangeStateTo(States.ALLOWING_EX_TRANSITION_MEMBERS_TO_DISCARD);
            }
        }


        private IEnumerator WhenAllowingTransitionMembersToDiscard()
        {
            AllowPlayerToDiscard(_taker.Value);
            AllowPlayerToDiscard(_giver.Value);
            StartDefiningCardGiver();
            yield break;
        }


        private void AllowPlayerToDiscard(PlayerId player)
        {
            if (ShouldDiscard())
            {
                if (NeverDiscardBefore(player))
                {
                    InitDiscardFor(player.WrapInNewList());
                }
                else
                {
                    ResumeDiscard();
                }
            }

            bool ShouldDiscard() => _playerCards[player].Count > 0;
            void ResumeDiscard() => _discardActivities[player].UnPausing();
        }


        private bool NeverDiscardBefore(PlayerId playerId) => !_discardActivities.ContainsKey(playerId);


        private void InitDiscardFor(IEnumerable<PlayerId> players)
        {
            foreach (var player in players)
            {
                _routines.AddRoutine(TryToWinMatch(player));
            }
        }


        private IEnumerator TryToWinMatch(PlayerId playerId)
        {
            var discard = new PausableDiscardingActivity(playerId, _timeContext, _discardPolicy, _playerCards[playerId]);
            _discardActivities.Add(playerId, discard);
            yield return _discardActivities[playerId].StartExecute();
            // если закончил предыдущее, значит осталс€ без карт
            yield return WinningMatch(playerId);
        }


        private IEnumerator WinningMatch(PlayerId player)
        {
            if (_playerCards[player].Any())
            {
                throw new Exception($"” игрока '{player}' по-прежднему есть карты. ќн не мог выиграть матч.");
            }

            _playersLeft.Remove(player);
            if (_discardActivities.ContainsKey(player))
            {
                _discardActivities.Remove(player);
            }

            yield return new PlayerLostAllCardsEvent(player);
            yield return Helper.RaiseContiniousEvent(new PlayerNotLostMatchEvent(player), 100, _timeContext);
        }


        private IEnumerator WhenDefiningNextCardTaker()
        {
            yield return WhenDefiningCardTransferParticipant(
                lookingForGiver: false,
                minCardsAmount: 0,
                ignorePlayers: WhoCanNotBeTaker(),
                OnDefined: p => { _nextTaker = p; StartTellingPlayersWhatToDo(); },
                OnOnePlayerLeft: StartFinishing);


            HashSet<PlayerId> WhoCanNotBeTaker() => new HashSet<PlayerId>() { _nextGiver.Value }; 
        }


        private IEnumerator WhenDefiningNextCardGiver()
        {
            yield return WhenDefiningCardTransferParticipant(
                lookingForGiver: true,
                minCardsAmount: 1,
                ignorePlayers: WhoCanNotBeGiver(),
                OnDefined: p => { _nextGiver = p; StartDefiningCardTaker(); },
                OnOnePlayerLeft: StartFinishing);;


            HashSet<PlayerId> WhoCanNotBeGiver() => 
                _giver != null ? 
                new HashSet<PlayerId>() { _giver.Value } :  // один и тот же игрок не берет карты дважды подр€д 
                new HashSet<PlayerId>();
        }


        private IEnumerator WhenDefiningCardTransferParticipant(bool lookingForGiver, int minCardsAmount, HashSet<PlayerId> ignorePlayers, Action<PlayerId> OnDefined, Action OnOnePlayerLeft)
        {
            var checkedCandidates = new HashSet<PlayerId>();

            while (!OnePlayerLeft())
            {
                yield return null; // во избежание "фриза" по неосторожности

                var candidate = FindNextCandidate(considerCurrent: lookingForGiver);
                if (checkedCandidates.Contains(candidate) || ignorePlayers.Contains(candidate))
                {
                    continue;
                }

                bool failed = false;
                yield return PrepareForTransfer(candidate, minCardsAmount, OnFailed: () => failed = true);

                if (failed)
                {
                    checkedCandidates.Add(candidate);
                    continue;
                }

                OnDefined?.Invoke(candidate);
                yield break;
            }

            OnOnePlayerLeft?.Invoke();
        }


        private void StartDefiningCardTaker()
        {
            _stateChecker.GoFurtherIf(States.DEFINING_NEXT_CARDS_GIVER).ChangeStateTo(States.DEFINING_NEXT_CARDS_TAKER);
        }


        private void StartDefiningCardGiver()
        {
            _stateChecker.GoFurtherIf(States.ALLOWING_EX_TRANSITION_MEMBERS_TO_DISCARD).ChangeStateTo(States.DEFINING_NEXT_CARDS_GIVER);
        }


        private void StartTellingPlayersWhatToDo()
        {
            _stateChecker.GoFurtherIf(States.DEFINING_NEXT_CARDS_TAKER).ChangeStateTo(States.TELLING_PLAYERS_WHAT_TO_DO);
        }


        private void StartFinishing()
        {
            Trace.Assert(OnePlayerLeft());
            _stateChecker.GoFurtherIf(States.DEFINING_NEXT_CARDS_TAKER, States.DEFINING_NEXT_CARDS_GIVER).ChangeStateTo(States.FINISHING);
        }


        private IEnumerator PrepareForTransfer(PlayerId player, int minCardsAmount, Action OnFailed)
        {

            if (NeverDiscardBefore(player))
            {
                yield break;
            }

            if (IsFinished())
            {
                OnFailed?.Invoke();
                yield break;
            }

            yield return WaitTimeBeforeStopDiscarding(player, sec: AllowedFreeTime());

            if (IsFinished())
            {
                OnFailed?.Invoke();
                yield break;
            }

            if (DoesntHaveEnoughtCards()) // проверка в начале...
            {
                OnFailed?.Invoke();
                yield break;
            }

            Action Unpause = null;
            yield return PauseDiscardActivity(player, OnGivePossibilityToUnpause: f => Unpause = f);

            if (DoesntHaveEnoughtCards()) // ... и проверка в конце (на вс€кий случай)
            {
                Unpause?.Invoke();
                OnFailed?.Invoke();
            }

            bool DoesntHaveEnoughtCards() => _playerCards[player].Count() < minCardsAmount;
            bool IsFinished() => _playerCards[player].Count() <= 0 || !_discardActivities.ContainsKey(player) || _discardActivities[player].IsFinished;
            int AllowedFreeTime() => (_taker != null && _taker.Value.Equals(player)) ? SEC_FOR_FREE_TIME_AFTER_TAKING_CARDS : 0;
        }


        private IEnumerator WaitTimeBeforeStopDiscarding(PlayerId player, int sec)
        {
            var timeLeft = sec * 1000;
            while (timeLeft > 0 && PlayerRunsDiscMode())
            {
                yield return null;
                timeLeft -= _timeContext.TimeSincePrevUpdate;
            }

            bool PlayerRunsDiscMode() => !(!_discardActivities.ContainsKey(player) || _discardActivities[player].IsFinished || _discardActivities[player].IsPaused);
        }


        private IEnumerator PauseDiscardActivity(PlayerId player, Action<Action> OnGivePossibilityToUnpause)
        {
            var disc = _discardActivities[player]; 
            if (!disc.IsPaused)
            {
                disc.StartPausing();
            }
            while (!disc.IsPaused)
            {
                yield return null;
            }
            OnGivePossibilityToUnpause(disc.UnPausing);
        }


        private PlayerId FindNextCandidate(bool considerCurrent)
        {
            if (!(considerCurrent && IsValid(_playersCycle.Current)))
            {
                do { _playersCycle.MoveNext(); }
                while (!IsValid(_playersCycle.Current));
            }

            return _playersCycle.Current;


            bool IsValid(PlayerId p) => _allPlayers.Contains(p) && _playerCards[p].Any();
        }


        private bool OnePlayerLeft()
        {
            return _playersLeft.Count == 1;
        }


        private IEnumerator WhenFinishing()
        {
            yield return Helper.RaiseContiniousEvent(new PlayerLostMatchEvent(_playersLeft.Single()), 1000, _timeContext);
            _stateChecker.ChangeStateTo(States.FINISHED);
        }

    }

}
