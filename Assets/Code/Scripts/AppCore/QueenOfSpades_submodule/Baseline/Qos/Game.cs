using CommonTools;
using CommonTools.StatesManaging;
using Qos.Domain.Events;
using Qos.GameLogic.GameWorld;
using Qos.Validation.EventsFlow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace Qos
{
    public class Game : DisposableClass
    {
        public enum States { INITED, STARTED, FINISHED, DISPOSED }

        private readonly StateChecker<States> _stateChecker;
        private readonly IGameWorld _gameWorld;
        private readonly IInteractor _interactor;
#if DEBUG
        private readonly IEventsFlowValidator _eventsFlowValidator = new EventsFlowValidator();
#endif


        public bool IsFinished => _stateChecker.CurrentState.Equals(States.FINISHED);


        public Game(Func<IGameWorld> BuildGameWorld, Func<IInteractor> BuildInteractor)
        {
            _gameWorld = BuildGameWorld();
            _interactor = BuildInteractor();

            _stateChecker = new StateChecker<States>(States.INITED);
        }


        public async Task RunAsync(Action OnFinished)
        {
            _stateChecker.GoFurtherIf(States.INITED).ChangeStateTo(States.STARTED);
            await MainGameLoop();
            _stateChecker.GoFurtherIf(States.STARTED).ChangeStateTo(States.FINISHED);

            OnFinished?.Invoke();
        }


        private async Task MainGameLoop()
        {
            await Updater.LoopAsync(
                async (ToStopLoop) =>
                {
                    if (_interactor.ExitRequested)
                    {
                        ToStopLoop();
                        return;
                    }

                    var gameEvents = _gameWorld.Iterate().ToList();
#if DEBUG
                    ValidateEvents(gameEvents);
#endif
                    _interactor.DepictEvents(gameEvents);
                    await Task.Delay(16);
                });
        }


#if DEBUG
        private void ValidateEvents(List<IEvent> events)
        {
            bool allEventsAreValid = true;
            foreach (var ev in events)
            {
                allEventsAreValid &= ValidateEvent(ev);
            }

            if (!allEventsAreValid)
            {
                Logger.Error("Некоторые из игровых событий не прошли валидацию.");
            }
        }

        private bool ValidateEvent(IEvent @event)
        {
            var problems = _eventsFlowValidator.CheckNextEvent(@event);
            if (problems?.Any() ?? false)
            {
                Logger.Error($"Событие {@event} не прошло валидацию.");
                foreach (var p in problems)
                {
                    Logger.Error(p.Message);
                }
                return false;
            }
            return true;
        }
#endif

        protected override void OnDisposed()
        {
            base.OnDisposed();
            _gameWorld.Dispose();
            _interactor.Dispose();
        }
    }
}
