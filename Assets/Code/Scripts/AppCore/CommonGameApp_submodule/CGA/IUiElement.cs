using System;


namespace CGA.MainMenu.GUI
{
    /// <summary>
    /// Графический элемент Ui.
    /// </summary>
    public interface IUiElement : IDisposable
    {
        event Action<IUiElement> OnPositionChanged;

        /// <summary>
        /// Позиция на экране в нормированой системе координат (от 0 до 1).
        /// Центр координат (0,0) в нижнем левом углу экрана. 
        /// Ось Y смотрит вверх.
        /// </summary>
        Vector2D Position { get; set; }
        /// <summary>
        /// Размер на экране (ширина, высота) в нормированой системе координат (от 0 до 1).
        /// </summary>
        Vector2D Size { get; set; }
        bool LocalyHidden { get; set; }

    }

}