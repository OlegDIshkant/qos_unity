using CGA.MainMenu.GUI;
using CommonTools;
using GUI;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace GameScene
{
    public class DiscModeUI : DisposableClass, IDiscModeUi
    {
        private UnityGui _gui;

        private List<object> _createdObjects = new List<object>();


        public DiscModeUI()
        {
            _gui = new UnityGui("Discard Mode Control");
        }


        public IPreDiscModeControl EnablePreDiscModeControls()
        {
            ThrowErrorIfDisposed();
            return WithRemembering(new PreDiscModeControl(_gui));
        }


        public IDiscModeControl EnableDiscModeControls()
        {
            ThrowErrorIfDisposed();
            return WithRemembering(new DiscModeControl(_gui));
        }


        private T WithRemembering<T>(T obj)
        {
            _createdObjects.Add(obj);
            return obj;
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            foreach (var obj in _createdObjects)
            {
                if (obj is IDisposable dispObj)
                {
                    dispObj.Dispose();
                }
            }
            _gui.Dispose();
        }
    }







    public abstract class UiControl<T> : DisposableClass, IUiControl<T>
    {
        public event Action<T> OnResultKnown;

        private IGui _gui;
        private bool _eventsSubscribed = false;

        protected ImmutableHashSet<IButton> Buttons { get; private set; }

        public T FinalResult { get; private set; }

        public UiControl(IGui gui)
        {
            _gui = gui;
            SetUpButtons();
            SubscribeToButtonEvents();
        }


        private void SetUpButtons()
        {
            var builder = ImmutableHashSet.CreateBuilder<IButton>();
            foreach (var btn in CreateButtons(_gui))
            {
                builder.Add(btn);
            }
            Buttons = builder.ToImmutable();
        }


        protected abstract IEnumerable<IButton> CreateButtons(IGui gui);


        private void SubscribeToButtonEvents()
        {
            if (!_eventsSubscribed)
            {
                foreach (var btn in Buttons)
                {
                    btn.OnClicked += OnButtonPressed;
                }
                _eventsSubscribed = true;
            }
        }


        private void UnSubscribeFromButtonEvents()
        {
            if (_eventsSubscribed)
            {
                foreach (var btn in Buttons)
                {
                    btn.OnClicked -= OnButtonPressed;
                }
                _eventsSubscribed = false;
            }
        }


        private void OnButtonPressed(IButton button)
        {
            ThrowErrorIfDisposed();

            UnSubscribeFromButtonEvents();
            FinalResult = DefineResultByPressedButton(button);
            OnResultKnown?.Invoke(FinalResult);
        }


        protected abstract T DefineResultByPressedButton(IButton button);


        protected override void OnDisposed()
        {
            base.OnDisposed();

            UnSubscribeFromButtonEvents();

            foreach (var btn in Buttons)
            {
                btn.Dispose();
            }
            Buttons = null;

            _gui = null;
        }



    }




    public class PreDiscModeControl : UiControl<PreDiscModeControlResult>, IPreDiscModeControl
    {
        private IButton _enterDiscModeBtn;


        public PreDiscModeControl(IGui gui) : base(gui)
        {
        }


        protected override IEnumerable<IButton> CreateButtons(IGui gui)
        {
            _enterDiscModeBtn = gui.CreateButton();
            _enterDiscModeBtn.Title = "В режим СБРОСА";
            _enterDiscModeBtn.Position = new CGA.Vector2D(0.15f, 0.9f);
            yield return _enterDiscModeBtn;

            var dosGoDiscModeBtn = gui.CreateButton();
            dosGoDiscModeBtn.Title = "НЕ сбрасывать";
            dosGoDiscModeBtn.Position = new CGA.Vector2D(0.15f, 0.85f);
            yield return dosGoDiscModeBtn;
        }


        protected override PreDiscModeControlResult DefineResultByPressedButton(IButton button)
        {
            return button == _enterDiscModeBtn ? PreDiscModeControlResult.GO_TO_DISC_MODE : PreDiscModeControlResult.WONT_GO_DISC_MODE;
        }
    }




    public class DiscModeControl : UiControl<DiscModeControlResult>, IDiscModeControl
    {
        private IButton _confirmButton;


        public DiscModeControl(IGui gui) : base(gui)
        {
        }


        public bool ConfirmButtonEnabled
        {
            get => !_confirmButton.LocalyHidden;
            set => _confirmButton.LocalyHidden = !value;
        }


        protected override IEnumerable<IButton> CreateButtons(IGui gui)
        {
            _confirmButton = gui.CreateButton();
            _confirmButton.Title = "Сбросить";
            _confirmButton.Position = new CGA.Vector2D(0.15f, 0.9f);
            yield return _confirmButton;

            var leaveBtn = gui.CreateButton();
            leaveBtn.Title = "Назад";
            leaveBtn.Position = new CGA.Vector2D(0.15f, 0.85f);
            yield return leaveBtn;
        }


        protected override DiscModeControlResult DefineResultByPressedButton(IButton button) =>
            button == _confirmButton ? DiscModeControlResult.CONFIRM_CARDS : DiscModeControlResult.LEAVE_DISC_MODE;
    }



}