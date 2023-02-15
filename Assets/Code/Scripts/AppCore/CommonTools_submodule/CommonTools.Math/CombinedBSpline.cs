using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace CommonTools.Math
{
    /// <summary>
    /// ����, ��������� �� ���������� ��������.
    /// </summary>
    public class CombinedBSpline 
    {
        private readonly List<BSpline> _splines;

        private float _normTimePerSpline;

        public CombinedBSpline(IEnumerable<BSpline> splines)
        {
            _splines = splines.ToList();
            _normTimePerSpline = 1f / _splines.Count;
        }


        /// <summary>
        /// ������ ����� �� ������������� ���������.
        /// </summary>
        public Vector3 CalcPoint(float normTime)
        {
            var splineIndex = System.Math.Min(
                (int)(normTime / _normTimePerSpline),
                _splines.Count - 1);

            var localNormTime = (normTime - _normTimePerSpline * splineIndex) / _normTimePerSpline;

            return _splines[splineIndex].CalcPoint(localNormTime);
        }

    }
}
