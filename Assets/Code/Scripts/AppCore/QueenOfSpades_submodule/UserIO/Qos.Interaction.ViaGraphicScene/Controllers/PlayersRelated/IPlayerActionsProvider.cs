using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// ������������� ���������� � ���, ��� � ������ ������ ������� ������ ����������� � ��� ��� ���� �������,
    /// � �����, ��� ������ ���� � ��� ����������� � ���������� ������ �������.
    /// </summary>
    public interface IPlayerActionsProvider : IPlayerActionsDefiner // ����� �������� ����� ���������� ����� ��������������� ���������� � ������� ������� �������
    {
        IPlayerActionsDefiner PreviousActions { get; } // ����� ���� ���� ����� ��������������� ���������� � ��������� � ���������� ������ �������
    }


    /// <summary>
    /// ���������, ��� � ������������ ������ ������� ������ ���������� � ��� ��� ���� �������.
    /// </summary>
    /// <remarks>
    public interface IPlayerActionsDefiner
    {
        DictionaryData<PlayerId, PlayerAction> DictionaryOutput { get; }
    }
}