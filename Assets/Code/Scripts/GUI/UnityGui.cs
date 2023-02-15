using CGA.MainMenu.GUI;
using CommonTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUI
{
    /// <summary>
    /// Адаптер к графическому пользовательскому интерфейсу от Unity3D.
    /// </summary>
    public partial class UnityGui : DisposableClass, IGui
    {
        private readonly CanvasFrame _frame;

        public event Action<IButton> OnButtonCreated;


        public bool IsHidden 
        {
            get { ThrowErrorIfDisposed(); return _frame.IsHidden; }
            set { ThrowErrorIfDisposed(); _frame.IsHidden = value; }
        }

        public ReadOnlyCollection<IUiElement> Elements 
        {
            get { ThrowErrorIfDisposed(); return _frame.UiElements; } 
        }


        public UnityGui(string guiTitle)
        {
            _frame = UnityCanvas.CreateFrame(guiTitle);
            _frame.OnButtonCreated += Frame_OnButtonCreated;
        }


        private void Frame_OnButtonCreated(IButton btn) => OnButtonCreated?.Invoke(btn);


        protected override void OnDisposed()
        {
            base.OnDisposed();

            _frame.OnButtonCreated -= Frame_OnButtonCreated;
            _frame.Dispose();
        }

        public IButton CreateButton() => _frame.CreateButton();

        public IImage CreateImage() => _frame.CreateImage();
    }


    public partial class UnityGui
    {
        /// <summary>
        /// Вспомагательный клас для работы с канвасом Unity3D.
        /// </summary>
        internal static class UnityCanvas
        {
            private static readonly string MAIN_CANVAS_NAME = "Main Canvas";

            public static CanvasFrame CreateFrame(string frameTitle)
            {
                var canvas = PrepareCanvas();
                return new CanvasFrame(frameTitle, (RectTransform)canvas.transform);
            }


            private static Canvas PrepareCanvas()
            {
                return GetExistedCanvas() ?? CreateCanvas();
            }


            private static Canvas GetExistedCanvas()
            {
                return GameObject.Find(MAIN_CANVAS_NAME)?.GetComponent<Canvas>();
            }


            private static Canvas CreateCanvas()
            {
                var canvasGO = new GameObject(MAIN_CANVAS_NAME);

                var canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();

                var eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.AddComponent<EventSystem>();
                eventSystemGo.AddComponent<StandaloneInputModule>();

                return canvas;
            }

        }


        /// <summary>
        /// "Логический" канвас (на самом деле часть канваса Unity3D).
        /// </summary>
        internal class CanvasFrame : DisposableClass
        {
            public event Action<IButton> OnButtonCreated;

            private readonly RectTransform _frame;

            private List<IUiElement> _uiElements = new List<IUiElement>();

            private bool p_isHidden = false;
            private ReadOnlyCollection<IUiElement> p_uiElements;


            public ReadOnlyCollection<IUiElement> UiElements
            {
                get { ThrowErrorIfDisposed(); return p_uiElements; }
            }

            public bool IsHidden
            {
                get { ThrowErrorIfDisposed(); return p_isHidden;  }
                set 
                { 
                    ThrowErrorIfDisposed();
                    if (p_isHidden == value) return;
                    p_isHidden = value;
                    _frame.gameObject.SetActive(!p_isHidden);
                }
            }


            public CanvasFrame(string frameTitle, RectTransform canvasTransform)
            {
                p_uiElements = new ReadOnlyCollection<IUiElement>(_uiElements);

                var frameGO = new GameObject(frameTitle);
                _frame = frameGO.AddComponent<RectTransform>();
                _frame.SetParent(canvasTransform);

                _frame.anchorMin = Vector2.zero;
                _frame.anchorMax = new Vector2(1, 1);
                _frame.offsetMax = _frame.offsetMin = Vector2.zero;
            }


            public IButton CreateButton()
            {
                ThrowErrorIfDisposed();

                var newBtn = new UnityGuiButton(OnUiElementDisposed);
                PutOnFrame(newBtn);
                _uiElements.Add(newBtn);
                OnButtonCreated?.Invoke(newBtn);
                return newBtn;
            }


            public IImage CreateImage()
            {
                ThrowErrorIfDisposed();

                var newImg = new UnityGuiImage(OnUiElementDisposed);
                PutOnFrame(newImg);
                _uiElements.Add(newImg);

                return newImg;
            }



            private void PutOnFrame(UnityGuiElement element)
            {
                element.SetParent(_frame);
            }


            private void OnUiElementDisposed(IUiElement uiElement)
            {
                _uiElements.Remove(uiElement);
            }


            protected override void OnDisposed()
            {
                base.OnDisposed();

                foreach (var element in _uiElements.ToList()) // Копируем в список, чтобы избежать ошибки чтения мутирующей коллекции
                {
                    element.Dispose();
                }
                p_uiElements = null;
                GameObject.Destroy(_frame.gameObject);
            }

        }
    }
}
