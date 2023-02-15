using CGA.MainMenu.GUI;
using System;
using UnityEngine.UI;


namespace GUI
{
    internal class UnityGuiButton : UnityGuiElement, IButton
    {
        private event Action<IButton> _OnClicked;
        public event Action<IButton> OnClicked
        {
            add { ThrowErrorIfDisposed(); _OnClicked += value; }
            remove { ThrowErrorIfDisposed(); _OnClicked -= value; }
        }


        private Text Text { get; set; }
        private Button ClickableComponent { get; set; }


        public string Title 
        { 
            get { ThrowErrorIfDisposed(); return Text.text; }
            set { ThrowErrorIfDisposed(); Text.text = value; }
        }


        public UnityGuiButton(Action<IUiElement> WhenDisposed)
            : base ("Prefabs/GuiElements/Common/SimpleButton", WhenDisposed)
        {
            Text = GameObj.GetComponentInChildren<Text>();
            ClickableComponent = GameObj.GetComponent<Button>();
            Title = "";

            ClickableComponent.onClick.AddListener(OnClick);
        }


        private void OnClick()
        {
            _OnClicked?.Invoke(this);
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            _OnClicked = null;
            Text = null;
            ClickableComponent = null;
        }
    }
}
