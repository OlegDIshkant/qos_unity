using System.Collections.Generic;
using System.Numerics;


namespace CommonTools.Math
{
    /// <summary>
    /// Трёхмерный сплайн.
    /// </summary>
    public class BSpline 
    {
        private readonly TinySpline.BSpline _spline;

        public BSpline(List<Vector3> points)
        {
            uint degree = points.Count <= 2 ? 1u : 2u; 

            _spline = new TinySpline.BSpline((uint)points.Count, 3, degree, TinySpline.BSpline.Type.Clamped);

            for (uint i = 0; i < points.Count; i++)
            {
                _spline.SetControlPointVec3At(i, ToVec3(points[(int)i]));
            }
        }

        /// <summary>
        /// Узнать точку по нормированому параметру.
        /// </summary>
        public Vector3 CalcPoint(float normValue)
        {
            return FromVec3(_spline.Eval(normValue).ResultVec3());
        }


        private TinySpline.Vec3 ToVec3(Vector3 original)
        {
            return new TinySpline.Vec3(original.X, original.Y, original.Z);
        }


        private Vector3 FromVec3(TinySpline.Vec3 original)
        {
            return new Vector3((float)original.X, (float)original.Y, (float)original.Z);
        }
    }
}
