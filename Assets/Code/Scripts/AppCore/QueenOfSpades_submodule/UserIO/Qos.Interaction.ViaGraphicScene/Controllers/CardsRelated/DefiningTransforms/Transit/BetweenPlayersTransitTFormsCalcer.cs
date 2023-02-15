using CommonTools;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет промежуточные <see cref="Transform"/> так, чтобы они описывали путь из колоды одного игрока в колоду другого.
    /// </summary>
    public class BetweenPlayersTransitTFormsCalcer : TransitTFormsCalcer
    {
        private CombinedBSpline _path;


        public BetweenPlayersTransitTFormsCalcer(Transform initial, Transform target) :
            base(initial, target)
        {
            RecalcPath();
        }


        public override Transform Calc(NormValue normTime)
        {
            var position = _path.CalcPoint(normTime.AsFloat);
            var rotation = Quaternion.Lerp(InitialTForm.Rotation, TargetTForm.Rotation, normTime.SqueezeToCenter(0.35f).AsFloat);
            var scale = Vector3.Lerp(InitialTForm.Scale, TargetTForm.Scale, normTime.AsFloat);

            return new Transform(position, rotation, scale);
        }


        protected override void OnTargetTformChanged()
        {
            RecalcPath();
        }


        private void RecalcPath()
        {
            _path = new CombinedBSpline(PathParts().ToList());
        }


        private IEnumerable<BSpline> PathParts()
        {
            var start = InitialTForm.Position;
            var end = TargetTForm.Position;
            var middle_1 = start + Vector3.UnitY * 3f;
            var middle_3 = end + Vector3.UnitY * 3f;
            var middle_2 = middle_1 + (middle_3 - middle_1) * 0.5f + Vector3.UnitY * 2;

            yield return new BSpline(new List<Vector3>() { start, middle_1 });
            yield return new BSpline(new List<Vector3>() { middle_1, middle_2, middle_3 });
            yield return new BSpline(new List<Vector3>() { middle_3, end });
        }
    }
}