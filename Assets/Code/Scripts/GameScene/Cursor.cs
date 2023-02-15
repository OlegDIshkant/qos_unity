using Qos.Interaction.ViaGraphicScene;
using UnityEngine;
using Utils;


namespace GameScene
{
    internal class Cursor : ICursor
    {
        private enum ButtonState { UNPRESSED, DOWN, PRESSED, UP }

        private ButtonState _leftButtonState = ButtonState.UNPRESSED;
        private Vector3 _cursorPosition;

        public System.Numerics.Vector2 NdcPosition { get; private set; }

        private bool _positionChanged = false;
        public bool PositionChanged => _positionChanged;

        public bool LeftClick => _leftButtonState == ButtonState.DOWN;


        public void Update()
        {
            UpdateCursorPosition();
            UpdateButtonState();
        }


        private void UpdateCursorPosition()
        {
            if (_cursorPosition == Input.mousePosition)
            {
                _positionChanged = false;
            }
            else
            {
                _cursorPosition = Input.mousePosition;
                NdcPosition = ToNdc(_cursorPosition);
                _positionChanged = true;
            }
        }


        // "Input.GetMouseButtonDown(0);" работает нечетко, поэтому написано своё решение
        private void UpdateButtonState()
        {
            bool isActiveNow = Input.GetMouseButton(0);

            switch (_leftButtonState)
            {
                case ButtonState.UNPRESSED:     if (isActiveNow) _leftButtonState = ButtonState.DOWN; break;
                case ButtonState.DOWN:          _leftButtonState = isActiveNow ? ButtonState.PRESSED : ButtonState.UP; break;
                case ButtonState.PRESSED:       if (!isActiveNow) _leftButtonState = ButtonState.UP; break;
                case ButtonState.UP:            _leftButtonState = isActiveNow ? ButtonState.DOWN : ButtonState.UNPRESSED; break;
                default:                        throw new System.Exception();
            }
        }


        private System.Numerics.Vector2 ToNdc(Vector3 mousePosition)
        {
            var normMousePos = new Vector2(
                mousePosition.x / Screen.width,
                mousePosition.y / Screen.height);

            var ndcPoint = (normMousePos - new Vector2(0.5f, 0.5f)) * 2;

            return new System.Numerics.Vector2(
                Mathf.Clamp(ndcPoint.x, -1f, 1f),   // на всякий случай обрезаем
                Mathf.Clamp(ndcPoint.y, -1f, 1f));
        }
    }
}
