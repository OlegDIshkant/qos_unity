

namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{

    /// <summary>
    /// ��������� ������������ ���� ��� ������, ����� ������� ����� (������������) �������� ����� ����� ��������� ���.
    /// </summary>
    internal class CamOriented_SelectKnown_CardTFormsCalcer : ForSelect_CardTFormsCalcer
    {
        public CamOriented_SelectKnown_CardTFormsCalcer() :
            base(new Config(lookToCam: true))
        {
        }
    }
}