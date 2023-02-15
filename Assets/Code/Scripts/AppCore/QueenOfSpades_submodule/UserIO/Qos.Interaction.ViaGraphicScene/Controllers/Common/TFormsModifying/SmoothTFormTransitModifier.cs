using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// ������ �������� ����� "�����" ����������� ����������� � ������������ ��������� ����� ��������.
    /// </summary>
    public class SmoothTFormTransitModifier<Key> : TFormsModifier<Key>
    {
        public SmoothTFormTransitModifier(
            Contexts context,
            float smoothSpeed,
            IDictionaryDataProvider<Key, Transform> originalTFormsProvider) :
            base(
                context,
                originalTFormsProvider,
                new AllwaysModifyStrategy(),
                new SmoothTransitModifyStrategy<Key>(context.TimeContext, smoothSpeed))
        {
        }
    }
}