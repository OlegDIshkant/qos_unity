using CGA.MainMenu;
using CommonTools.StatesManaging;
using System;
using System.Threading.Tasks;


namespace CGA
{
    /// <summary>
    /// App that runs a game and lateral things (like main menu etc).
    /// </summary>
    public class GameApp : IApp
    {
        private readonly AbstractMainMenu _mainMenu;
        private readonly IAppCoreFactory _appCoreFactory;

        private IAppCore _appCore;
        private StateChecker<AppState> _stateChecker = new StateChecker<AppState>(AppState.JUST_CREATED);

        public AppState CurrentState => (AppState)_stateChecker.CurrentState;


        public GameApp(AbstractMainMenu mainMenu, IAppCoreFactory appCoreFactory)
        {
            _mainMenu = mainMenu ?? throw new NullReferenceException();
            _appCoreFactory = appCoreFactory ?? throw new NullReferenceException();
        }


        public async Task LaunchAsync()
        {
            _stateChecker.GoFurtherIf(AppState.JUST_CREATED).ChangeStateTo(AppState.RUNNING);
            await RunningAppLoopAsync(); // Let outer code to catch unhandled exceptions
            _stateChecker.GoFurtherIf(AppState.RUNNING).ChangeStateTo(AppState.FINISHED);
        }


        private async Task RunningAppLoopAsync()
        {

            while (true)
            {
                var toLaunchAppCore = false;

                await RunMainMenuAsync(
                    OnRequestAppCoreLaunch: () => toLaunchAppCore = true,
                    OnRequestExit: () => { /* Если не выбрана опция "начать игру", считаем, что выбран выход. */ });

                if (toLaunchAppCore)
                {
                    await RunAppCore();
                }
                else
                {
                    break;
                }
            }
        }


        async Task RunMainMenuAsync(Action OnRequestAppCoreLaunch, Action OnRequestExit)
        {
            if (_mainMenu.CurrentState == MainMenuState.HIDDEN)
            {
                _mainMenu.UnHide();
            }
            else
            {
                _mainMenu.Launch();
            }

            while (
                _mainMenu.CurrentState != MainMenuState.NEW_GAME_REQUESTED &&
                _mainMenu.CurrentState != MainMenuState.EXIT_REQUESTED)
            {
                await Task.Delay(500);
            }

            if (_mainMenu.CurrentState == MainMenuState.NEW_GAME_REQUESTED)
            {
                OnRequestAppCoreLaunch();
                _mainMenu.Hide();
            }
            else if (_mainMenu.CurrentState == MainMenuState.EXIT_REQUESTED)
            {
                OnRequestExit();
            }
            else
            {
                throw new Exception();
            }
        }


        async Task RunAppCore()
        {
            _appCore = _appCoreFactory.GenerateAppCore();
            _appCore.Launch();

            while (_appCore.State != AppCoreState.FINISHED)
            {
                await Task.Delay(1_000);
            }

            _appCore.Dispose();
            _appCore = null;
        }

    }
}
