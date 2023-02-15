using CommonTools;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет промежуточные <see cref="Transform"/> так, чтобы они описывали путь карты в "кучу".
    /// </summary>
    public class ToHeapTransitTFormsCalcer : TransitTFormsCalcer
    {
        private BSpline _path;


        public ToHeapTransitTFormsCalcer(Transform initial, Transform target) : 
            base(initial, target)
        {
            RecalcPath();
        }


        public override Transform Calc(NormValue normTime)
        {
            var position = _path.CalcPoint(normTime.AsFloat);
            var rotation = Quaternion.Lerp(InitialTForm.Rotation, TargetTForm.Rotation, normTime.SqueezeToCenter(0.3f).AsFloat);
            var scale = Vector3.Lerp(InitialTForm.Scale, TargetTForm.Scale, normTime.AsFloat);

            return new Transform(position, rotation, scale);
        }


        protected override void OnTargetTformChanged()
        {
            RecalcPath();
        }


        private void RecalcPath()
        {
            _path = new BSpline(PathPoints().ToList());
        }


        private IEnumerable<Vector3> PathPoints()
        {
            /*var start = InitialTForm.Position;
            var end = TargetTForm.Position;
            var start2end = end - start;
            var middle_1 = start + Vector3.UnitY * 2;
            var middle_2 = start + start2end * 0.5f + Vector3.UnitY * 2;
            var middle_3 = end + Vector3.UnitY * 4;*/


            var start = InitialTForm.Position;
            var end = TargetTForm.Position;
            var middle_1 = start + Vector3.UnitY * 3;
            var middle_3 = end + Vector3.UnitY * (middle_1.Y - end.Y);
            var middle_2 = (middle_1 + middle_3) / 2 + Vector3.UnitY * 3;

            yield return start;
            yield return middle_1;
            yield return middle_2;
            yield return middle_3;
            yield return end;
        }
    }
}