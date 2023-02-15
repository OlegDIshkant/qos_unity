using CGA.MainMenu.GUI;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CGA.MainMenu
{
    public class MainMenuWithGUI : AbstractMainMenu
    {
        public override event Action<MainMenuState, MainMenuState> OnStateChanged;

        private readonly IGui _gui;

        private IButton _startGameBtn;
        private IButton _exitAppBtn;

        private  MainMenuState _currentState = MainMenuState.JUST_CREATED;
        public override MainMenuState CurrentState { get => _currentState; }


        private List<IButton> Buttons => _gui.Elements.Where(e => e is IButton).Select(e => (IButton)e).ToList();


        public MainMenuWithGUI(IGui gui) : base()
        {
            _gui = gui ?? throw new ArgumentNullException(nameof(gui));
        }


        private void ChangeState(MainMenuState newState)
        {
            var prevState = CurrentState;
            _currentState = newState;
            OnStateChanged?.Invoke(prevState, newState);
        }


        public override void Launch()
        {
            ChangeState(MainMenuState.LAUNCHED);
            SetUpGui();
        }


        private void SetUpGui()
        {
            SetUpBackground();
            SetUpButtons();
        }


        private void SetUpBackground()
        {
            var background = _gui.CreateImage();
            background.Position = new Vector2D(0.5f, 0.5f);
            background.Size = new Vector2D(1, 1);
        }


        private void SetUpButtons()
        {
            _startGameBtn = _gui.CreateButton();
            _exitAppBtn = _gui.CreateButton();

            _startGameBtn.Position = new Vector2D(0.33f, 0.5f); // левая кнопка
            _exitAppBtn.Position = new Vector2D(0.66f, 0.5f); // правая кнопка

            _startGameBtn.Title = "Новая Игра";
            _exitAppBtn.Title = "Выход";

            StartListenAllGuiButtons();
        }


        private void StartListenAllGuiButtons()
        {
            foreach (var btn in Buttons)
                btn.OnClicked += GuiButton_OnClicked;
        }


        private void StopListenAllGuiButtons()
        {
            foreach (var btn in Buttons)
                btn.OnClicked -= GuiButton_OnClicked;
        }


        private void GuiButton_OnClicked(IButton clickedBtn)
        {
            StopListenAllGuiButtons();

            if (clickedBtn == _startGameBtn)
            {
                ChangeState(MainMenuState.NEW_GAME_REQUESTED);
            }
            else if (clickedBtn == _exitAppBtn)
            {
                ChangeState(MainMenuState.EXIT_REQUESTED);
            }
            else
            {
                throw new Exception("Unexpected button has been clicked.");
            }
        }


        public override void Hide()
        {
            ChangeState(MainMenuState.HIDDEN);
            _gui.IsHidden = true;
        }


        public override void UnHide()
        {
            if (CurrentState != MainMenuState.HIDDEN)
            {
                throw new InvalidOperationException("Can not unhide main menu that had not been hidden.");
            }
            ChangeState(MainMenuState.LAUNCHED);
            StartListenAllGuiButtons();
            _gui.IsHidden = false;
        }
    }
}
