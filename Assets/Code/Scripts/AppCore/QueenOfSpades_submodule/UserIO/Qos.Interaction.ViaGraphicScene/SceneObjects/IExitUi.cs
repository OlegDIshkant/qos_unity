using System;


namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{
    /// <summary>
    /// Ui, �� ������� ����������� ���������� ��� ������ � ������� ����.
    /// </summary>
    public interface IExitUi
    {
        event Action<IExitUi> OnExitRequested;
    }
}