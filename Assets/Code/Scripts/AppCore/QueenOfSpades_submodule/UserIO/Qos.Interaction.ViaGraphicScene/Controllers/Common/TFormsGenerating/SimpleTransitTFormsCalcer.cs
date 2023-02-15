using CommonTools;
using CommonTools.Math;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// ��������� ������������� <see cref="Transform"/> ���, ����� ��� ��������� ������������� ���� � ����.
    /// </summary>
    public class SimpleTransitTFormsCalcer : TransitTFormsCalcer
    {
        public SimpleTransitTFormsCalcer(Transform initial, Transform target) : 
            base(initial, target)
        {
        }


        public override Transform Calc(NormValue normTime)
        {
            return Transform.Lerp(InitialTForm, TargetTForm, normTime.AsFloat);
        }


        protected override void OnTargetTformChanged()
        {

        }
    }
}