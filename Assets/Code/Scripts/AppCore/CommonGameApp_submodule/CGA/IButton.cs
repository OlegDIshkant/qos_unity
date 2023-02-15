using System;


namespace CGA.MainMenu.GUI
{
    public interface IButton : IUiElement
    {
        event Action<IButton> OnClicked;

        /// <summary>
        /// Надпись на кнопке.
        /// </summary>
        string Title { get; set; }
    }
}
