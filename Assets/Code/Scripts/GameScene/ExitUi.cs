using CommonTools;
using GUI;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;


namespace GameScene
{
    public class ExitUi : DisposableClass, IExitUi
    {
        private UnityGui _gui;
        private CGA.MainMenu.GUI.IButton _exitBtn;

        public event Action<IExitUi> OnExitRequested;

        public ExitUi()
        {
            _gui = new UnityGui("Exit (To Main Menu) Control");

            _exitBtn = _gui.CreateButton();
            _exitBtn.Position = new CGA.Vector2D(0.9f, 0.05f);
            _exitBtn.Title = "Выход";
            _exitBtn.OnClicked += ExitBtn_OnClicked;
        }


        private void ExitBtn_OnClicked(CGA.MainMenu.GUI.IButton exitBtn)
        {
            _exitBtn.OnClicked -= ExitBtn_OnClicked;
            OnExitRequested?.Invoke(this);
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            _exitBtn.OnClicked -= ExitBtn_OnClicked;  
            _exitBtn = null;

            _gui.Dispose();
            _gui = null;
        }
    }
}