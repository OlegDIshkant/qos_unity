using CGA;
using CommonTools;
using CommonTools.StatesManaging;
using Qos;
using Qos.GameLogic.GameWorld;
using System;


namespace QosGameApp
{
    /// <summary>
    /// The main "screen" of "Queen of spades" game.
    /// </summary>
    internal class QosGame : DisposableClass, IAppCore
    {
        public event Action<AppCoreState, AppCoreState> OnStateChanged;

        private readonly Func<IGameWorld> _BuildGameWorld;
        private readonly Func<IInteractor> _BuildInteractor;

        private Game _game;
        private StateChecker<AppCoreState> _stateChecker;

        public AppCoreState State => (AppCoreState)_stateChecker.CurrentState;


        public QosGame(Func<IGameWorld> BuildGameWorld, Func<IInteractor> BuildInteractor)
        {
            _BuildGameWorld = BuildGameWorld;
            _BuildInteractor = BuildInteractor;

            _stateChecker = new StateChecker<AppCoreState>(AppCoreState.JUST_CREATED, disposedState: AppCoreState.DISPOSED);
        }


        public void Launch()
        {
            ChangeStateToStarted();

            _game = new Game(_BuildGameWorld, _BuildInteractor);
            var _ = _game.RunAsync(OnFinished: () => ChangeStateToFinished());

        }


        void ChangeStateToStarted()
        {
            _stateChecker.GoFurtherIf(AppCoreState.JUST_CREATED).ChangeStateTo(AppCoreState.LAUNCHED);
            OnStateChanged?.Invoke(AppCoreState.JUST_CREATED, AppCoreState.LAUNCHED);
        }


        void ChangeStateToFinished()
        {
            _stateChecker.GoFurtherIf(AppCoreState.LAUNCHED).ChangeStateTo(AppCoreState.FINISHED);
            OnStateChanged?.Invoke(AppCoreState.LAUNCHED, AppCoreState.FINISHED);
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            _stateChecker.GoFurtherIf(AppCoreState.FINISHED).ChangeStateTo(AppCoreState.FINISHED);
            OnStateChanged?.Invoke(AppCoreState.FINISHED, AppCoreState.DISPOSED);

            _game.Dispose();
            _game = null;
        }

    }
}
