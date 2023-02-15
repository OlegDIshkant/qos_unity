using CommonTools.Math;
using CommonTools.Math.Geometry2D;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System;
using System.Linq;
using System.Numerics;
using static CommonTools.Math.NDCPointToPlaneSolver;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет положения, вращения и так далее для карт главного игрока.
    /// </summary>
    internal class MainPlayerIdleCardTFormsCalcer : ITFormsCalcer, IWithCamParams
    {
        private readonly float DIST_BETWEEN_CARDS = 0.03f;


        public CameraParams CamParams { get; set; }

        protected virtual float ScaleFactor => 0.3f;
        protected virtual float MaxDegBetweenCards => 6f;
        protected virtual Vector3 LocalPositionOffset => Vector3.Zero;


        public Transform[] Calc(int cardsCount)
        {
            CardsPlaneCalcer.FindMatricies(CamParams, out var cardsPlaneMMatrix, out var invCardsPlaneMMatrix);


            var result = new Transform[cardsCount];
            var cardPoints = GenCardPointsInCardsPlane(CamParams, invCardsPlaneMMatrix, cardsCount);

            var middleIndex = (int)cardPoints.Length / 2;
            for (int i = 0; i < cardsCount; i++)
            {
                var position = CalcWorldPosition(cardsPlaneMMatrix, cardPoints[i], i);
                var rotation = CalcWorldRotation(cardsPlaneMMatrix, middleIndex - i);
                var scale = CalcWorldScale();
                result[i] = new Transform(position, rotation, scale);
            }

            return result;



        }


        void FindKeyPointsInCardsPlaneSpace(
            CameraParams camParams, 
            Matrix4x4 invCardsPlaneMMatrix, 
            out Vector3 centerPoint, 
            out Vector3 upperPoint, 
            out Vector3 rightestPoint)
        {
            if (!TryProject(new Vector2(0, -0.8f), out centerPoint)) { throw new Exception(); }
            if (!TryProject(new Vector2(0, -0.2f), out upperPoint)) { throw new Exception(); }
            if (!TryProject(new Vector2(0.9f, 0), out rightestPoint)) { throw new Exception(); }


            bool TryProject(Vector2 normScreenPoint, out Vector3 result)
            {
                return TrySolveForLocalPlane(
                    invCardsPlaneMMatrix,
                    GeometryUtils.PlaneZ(),
                    normScreenPoint,
                    camParams,
                    out result);
            }
        }


        private Vector2[] GenCardPointsInCardsPlane(CameraParams camParams, Matrix4x4 inverseModelMatrix, int pointsAmount)
        {
            FindKeyPointsInCardsPlaneSpace(camParams, inverseModelMatrix, out var centerPoint, out var upperPoint, out var rightestPoint);
            var elipseHalfHeight = (upperPoint.Y - centerPoint.Y) * 1f;
            var elipseHalfWidth = (rightestPoint.X - centerPoint.X) * 1f;

            var elipsePointsCalculator = new ElipsePointsCalculator(elipseHalfWidth, elipseHalfHeight) { MaxAngle = MaxDegBetweenCards };
            return elipsePointsCalculator.CalcPoints(pointsAmount)
                .Select(p => p + centerPoint.ToVector2())
                .ToArray();
        }


        private Vector3 CalcWorldPosition(Matrix4x4 modelMatrix, Vector2 planePoint, int index)
        {
            var localPosition = 
                new Vector4(planePoint.X, planePoint.Y, -index * DIST_BETWEEN_CARDS, 1) +
                new Vector4(LocalPositionOffset, 0);
            return modelMatrix.ApplyTo(localPosition).ToVector3();
        }


        private Quaternion CalcWorldRotation(Matrix4x4 modelMatrix, int relIndex)
        {
            var worldLookDir = modelMatrix.ApplyTo(Vector4.UnitZ).ToVector3(); 
            var worldUpDir = modelMatrix.ApplyTo(new Vector4(relIndex * 1, 1, 0, 1)).ToVector3(); 
            return GeometryUtils.InitialRotation(worldLookDir, worldUpDir);
        }


        private Vector3 CalcWorldScale()
        {
            var scaleFactor = ScaleFactor; // Пока захардкодим, но нужно определить нормальный механизм вычисления
            return new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }


}
