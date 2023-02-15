using CommonTools;
using System;
using System.Collections.ObjectModel;


namespace CGA.MainMenu.GUI
{
    /// <summary>
    /// Abstract graphical user interface.
    /// </summary>
    public interface IGui : ICustomDisposable 
    {
        event Action<IButton> OnButtonCreated;

        /// <summary>
        /// Allows to hide/unhide itself and all its elements.
        /// </summary>
        bool IsHidden { get; set; }

        /// <summary>
        /// All created Ui Elements.
        /// </summary>
        ReadOnlyCollection<IUiElement> Elements { get; }

        /// <summary>
        /// Generates button and add it to the ui.
        /// </summary>
        IButton CreateButton();

        /// <summary>
        /// Generates image and add it to the ui.
        /// </summary>
        IImage CreateImage();

    }
}
