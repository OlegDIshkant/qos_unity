using CommonTools;
using CommonTools.StatesManaging;
using Qos.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Qos.GameLogic.GameWorld.Activities
{
    /// <summary>
    /// Процесс, схожий с <see cref="DiscardingActivity"/>, но который ведеться до потери всех карт. 
    /// Также его можно прерывать на время и возобновлять после.
    /// </summary>
    internal class PausableDiscardingActivity : AbstractPlayerActivity
    {
        private enum States
        {
            UNSTARTED,
            PLAYING,
            PAUSING,
            PAUSED,
            FINISHED
        }

        private readonly StateChecker<States> _stateChecker = new StateChecker<States>(States.UNSTARTED);
        private readonly HashSet<CardId> _playersCards;
        private readonly PlayerId _player;
        private readonly TimeContext _timeContext;
        private readonly ICardsDiscardPolicy _discardPolicy;

        private DiscardingActivity _discActivity;
        private IEnumerator _discActivityExecuting;


        public bool IsPaused => _stateChecker.CurrentState.Equals(States.PAUSED);

        public bool IsFinished => _stateChecker.CurrentState.Equals(States.FINISHED);


        public PausableDiscardingActivity(
            PlayerId player, 
            TimeContext timeContext, 
            ICardsDiscardPolicy discardPolicy,
            HashSet<CardId> playersCardsToModify)
        {
            _player = player;
            _timeContext = timeContext;
            _discardPolicy = discardPolicy;
            _playersCards = playersCardsToModify;
        }


        public void StartPausing()
        {
            _stateChecker.GoFurtherIf(States.PLAYING).ChangeStateTo(States.PAUSING);
            _discActivity.Cancel();
        }


        public void UnPausing()
        {
            _stateChecker.GoFurtherIf(States.PAUSED).ChangeStateTo(States.PLAYING);

            if (NoMoreCardsLeft())
            {
                _stateChecker.ChangeStateTo(States.FINISHED);
            }
            else
            {
                BeginNewDiscActivity();
            }
        }


        private void BeginNewDiscActivity()
        {
            _discActivity = new DiscardingActivity(_player, _timeContext, _discardPolicy, _playersCards);
            _discActivityExecuting = _discActivity.StartExecute();
            
        }


        protected override IEnumerator Executing()
        {
            _stateChecker.ChangeStateTo(States.PLAYING);
            BeginNewDiscActivity();

            while (true)
            {
                switch (_stateChecker.CurrentState)
                {
                    case States.PLAYING:    yield return WhenPlaying(); break;
                    case States.PAUSING:    yield return WhenPausing(); break;
                    case States.PAUSED:     yield return null;  break;
                    case States.FINISHED:   yield break;
                    default: throw new Exception();
                }
            }
        }


        private IEnumerator WhenPlaying()
        {
            if (_discActivityExecuting.MoveNext())
            {
                yield return _discActivityExecuting.Current;
            }
            else if (NoMoreCardsLeft())
            {
                _stateChecker.ChangeStateTo(States.FINISHED);
            }
            else
            {
                _stateChecker.ChangeStateTo(States.PAUSED);
            }
        }


        private IEnumerator WhenPausing()
        {
            if (_discActivityExecuting.MoveNext())
            {
                yield return _discActivityExecuting.Current;
            }
            else if (NoMoreCardsLeft())
            {
                _stateChecker.ChangeStateTo(States.FINISHED);
            }
            else
            {
                _stateChecker.ChangeStateTo(States.PAUSED);
            }
        }


        public bool NoMoreCardsLeft()
        {
            return !_playersCards.Any();
        }
    }
}