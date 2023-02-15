

namespace CGA.MainMenu.GUI
{
    /// <summary>
    /// Некое изображение на Ui.
    /// </summary>
    public interface IImage : IUiElement
    {
        /// <param name="imageKey">
        /// Относительный (!) путь к изображению. (Абсолютный путь зависит от конкретной имплементации.)
        /// </param>
        void SetImage(string imageKey);

        float Alpha { get; set; }
    }
}