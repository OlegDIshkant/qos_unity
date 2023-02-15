using CGA;
using CGA.MainMenu.GUI;
using CommonTools;
using System;
using UnityEditor;
using UnityEngine;


namespace GUI
{
    public class UnityGuiElement : DisposableClass, IUiElement
    {

        private Action<IUiElement> _WhenDisposed;


        private Vector2D _position;
        public Vector2D Position
        {
            get { ThrowErrorIfDisposed(); return _position; }
            set
            {
                ThrowErrorIfDisposed();
                _position = value;
                RedrawAtPosition(value);
                _OnPositionChanged?.Invoke(this);
            }
        }


        public event Action<IUiElement> _OnPositionChanged;
        public event Action<IUiElement> OnPositionChanged
        {
            add { ThrowErrorIfDisposed(); _OnPositionChanged += value; }
            remove { ThrowErrorIfDisposed(); _OnPositionChanged -= value; }
        }


        public Vector2D Size 
        {
            get { ThrowErrorIfDisposed(); return new Vector2D(Transform.rect.width / Screen.width, Transform.rect.height / Screen.height); }
            set { ThrowErrorIfDisposed(); Transform.sizeDelta = new Vector2(value.X * Screen.width, value.Y * Screen.height); }
        }


        private RectTransform Transform { get; set; }

        public bool LocalyHidden
        {
            get => !GameObj.activeSelf;
            set => GameObj.SetActive(!value);
        }

        protected GameObject GameObj => Transform.gameObject;


        public UnityGuiElement(string pathToPrefab, Action<IUiElement> WhenDisposed)
        {
            _WhenDisposed = WhenDisposed;

            var prototype = Resources.Load(pathToPrefab, typeof(UnityEngine.Object));
            var go = (GameObject)UnityEngine.Object.Instantiate(prototype);
            Transform = go.GetComponent<RectTransform>();
            RedrawAtPosition(new Vector2D(0, 0));
        }


        private void RedrawAtPosition(Vector2D position)
        {
            var pos = new Vector2(position.X, position.Y);
            Transform.anchorMax = pos;
            Transform.anchorMin = pos;
            Transform.anchoredPosition = Vector2.zero;
        }


        public void SetParent(RectTransform parent)
        {
            GameObj.transform.SetParent(parent);
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();

            _WhenDisposed?.Invoke(this);
            _WhenDisposed = null;

            _OnPositionChanged = null;

            GameObject.Destroy(GameObj);
            Transform = null;
        }

    }
}