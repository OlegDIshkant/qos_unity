using CommonTools.Math;
using CommonTools.Math.Geometry2D;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.ComponentModel;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет расположения для карт игрока такие, чтобы показать, что он ничего особенного с ними сейчас не делает.
    /// </summary>
    public class PlayerIdleTFormsCalcer : IWithPlayerTForm
    {
        protected virtual float MaxDegBetweenCards => 6f;

        public class Params
        {
            public Vector3 Scale { get; set; }

            public static Params Default =>
                new Params()
                {
                    Scale = new Vector3(1, 1, 1)
                };
        }

        private Vector4 CardPivotBias = new Vector4(0, - MIN_ELIPSE_SIDE / 2, MIN_ELIPSE_SIDE, 0);

        private static readonly float MIN_ELIPSE_SIDE = 2;
        private readonly Params _params;

        private Matrix4x4 _playerLocToWorldMatrix;


        public PlayerIdleTFormsCalcer(Transform playerTransform, Params @params)
        {
            ChangePlayerTForm(playerTransform);
            _params = @params;
        }


        public PlayerIdleTFormsCalcer(Transform playerTransform) :
            this(playerTransform, Params.Default)
        {

        }



        public void ChangePlayerTForm(Transform playerTForm)
        {
            _playerLocToWorldMatrix = playerTForm.CalcTRS();
        }


        /// <summary>
        /// Пересчитать положения карт игрока.
        /// </summary>
        public Transform[] Calc(int cardsAmount)
        {
            var result = new Transform[cardsAmount];

            var planePoints = GenPointsInHorizontalPlane(cardsAmount);
            int midIndex = cardsAmount / 2;
            for (int i = 0; i < cardsAmount; i++)
            {
                var position = CalcPosition(planePoints[i], i);
                var rotation = CalcRotation(midIndex - i);
                var scale = CalcScale();
                result[i] = new Transform(position, rotation, scale);
            }

            return result;
        }


        private Vector2[] GenPointsInHorizontalPlane(int pointsAmount)
        {
            var screenPointsCalculator = new ElipsePointsCalculator(
                elipseHalfWidth: MIN_ELIPSE_SIDE * 1.5f,
                elipseHalfHeight: MIN_ELIPSE_SIDE)
            {
                MaxAngle = MaxDegBetweenCards
            };

            return screenPointsCalculator.CalcPoints(pointsAmount);
        }


        private Vector3 CalcPosition(Vector2 planePoint, int index)
        {
            return _playerLocToWorldMatrix.ApplyTo(new Vector4(planePoint.X, planePoint.Y, index * 0.05f, 1) + CardPivotBias).ToVector3();
        }


        private Quaternion CalcRotation(int relIndex)
        {
            var cardForward = _playerLocToWorldMatrix.ApplyTo(new Vector4(0, 0, -1, 0)).ToVector3();
            var cardUp = _playerLocToWorldMatrix.ApplyTo(new Vector4(relIndex * .05f, 1, 0, 0)).ToVector3();
            return GeometryUtils.InitialRotation(cardForward, cardUp);
        }


        private Vector3 CalcScale()
        {
            return _params.Scale;
        }
    }
}
