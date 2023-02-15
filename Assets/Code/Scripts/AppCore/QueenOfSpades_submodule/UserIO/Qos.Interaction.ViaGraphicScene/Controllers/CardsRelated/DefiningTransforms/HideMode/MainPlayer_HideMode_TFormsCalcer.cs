using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ����������� ��������� ���� �������� ������ (������������) ���, ����� ��������� ��������, ��� ��� ������������ "�� ������ ����".
    /// </summary>
    internal class MainPlayer_HideMode_TFormsCalcer : MainPlayerIdleCardTFormsCalcer
    {
        protected override Vector3 LocalPositionOffset => new Vector3(0, -0.9f, 0);
    }
}