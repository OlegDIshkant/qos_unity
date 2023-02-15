using System;


namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{
    /// <summary>
    /// Ui, на котором расположено управление для выхода в главное меню.
    /// </summary>
    public interface IExitUi
    {
        event Action<IExitUi> OnExitRequested;
    }
}