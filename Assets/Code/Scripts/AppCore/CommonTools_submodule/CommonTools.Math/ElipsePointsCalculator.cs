using System.Numerics;

namespace CommonTools.Math.Geometry2D
{
    /// <summary>
    /// Генерирует точки, лежащие на верней части элипса.
    /// </summary>
    public class ElipsePointsCalculator
    {
        private static readonly float MAX_RANGE = 180;

        private const float DEG_TO_RAD = (float)System.Math.PI / 180f;

        private readonly float _a;
        private readonly float _b;

        private float p_maxAngle = MAX_RANGE;

        /// <summary>
        /// Представте два луча:
        /// один идет от центра элипса к точке 1 на элипсе,
        /// а второй идет от центра элипса к точке 2 на элипсе, причем точка 2 - ближайший сосед точки 1.
        /// Данный параметр задает максимальный разрешенный угол между подобными двумя лучами.
        /// Он позволяет вычисляемым точкам на элипсе не "расходится" далеко по окружности.
        /// </summary>
        public float MaxAngle
        {
            get => p_maxAngle; 
            set => p_maxAngle = System.Math.Max(value, 1);
        }


        public ElipsePointsCalculator(float radius)
            : this(radius, radius)
        {
        }


        public ElipsePointsCalculator(float elipseHalfWidth, float elipseHalfHeight)
        {
            _a = System.Math.Max(0, elipseHalfWidth);
            _b = System.Math.Max(0, elipseHalfHeight);
        }


        public Vector2[] CalcPoints(int pointsAmount)
        {
            CalcAnglesRad(pointsAmount, out var startAngle, out var angleStep);

            var result = new Vector2[pointsAmount];
            for (int i = 0; i < pointsAmount; i++)
            {
                result[i] = CalcPoint(startAngle + i * angleStep);
            }

            return result;
        }

        
        private float MaxDesiredRange(int pointsAmount)
        {
            return MaxAngle * (pointsAmount - 1);
        }


        private void CalcAnglesRad(int pointsAmount, out float startAngle, out float stepAngle)
        {
            var desiredRange = MaxDesiredRange(pointsAmount);
            var actualRange = desiredRange > MAX_RANGE ? MAX_RANGE : desiredRange;
            startAngle = (MAX_RANGE - actualRange) / 2f;
            stepAngle = (pointsAmount > 1) ? actualRange / (pointsAmount - 1) : 0;

            startAngle *= DEG_TO_RAD;
            stepAngle *= DEG_TO_RAD;
        }


        private Vector2 CalcPoint(float angleRad)
        {
            var x = (float)System.Math.Cos(angleRad) * _a;
            var y = (float)System.Math.Sqrt(1 - System.Math.Pow(x / _a, 2)) * _b;

            return new Vector2(x, y);
        }
    }
}
