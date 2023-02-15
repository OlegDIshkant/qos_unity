using CGA.MainMenu.GUI;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace GUI
{
    public class UnityGuiImage : UnityGuiElement, IImage
    {
        private Image _imageComponent;


        public UnityGuiImage(Action<IUiElement> WhenDisposed)
            : base ("Prefabs/GuiElements/Common/SimpleImage", WhenDisposed)
        {
            _imageComponent = GameObj.GetComponent<Image>();
            _imageComponent.preserveAspect = true;
        }

        public float Alpha 
        {
            get { ThrowErrorIfDisposed(); return _imageComponent.color.a;  }
            set { ThrowErrorIfDisposed(); var c = _imageComponent.color; c.a = value; _imageComponent.color = c; } 
        }


        public void SetImage(string imageKey)
        {
            _imageComponent.sprite = Resources.Load<Sprite>($"sprites/{imageKey}");
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            _imageComponent = null;
        }
    }
}