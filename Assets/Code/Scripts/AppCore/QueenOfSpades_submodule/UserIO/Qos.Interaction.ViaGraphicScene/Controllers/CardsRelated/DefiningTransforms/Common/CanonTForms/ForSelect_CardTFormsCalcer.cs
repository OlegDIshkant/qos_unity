using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System;
using System.Linq;
using System.Numerics;
using static CommonTools.Math.NDCPointToPlaneSolver;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет расположения для карт такие, чтобы все эти карты целиком расположились на экране и их можно было удобно выбрать курсором.
    /// </summary>
    internal class ForSelect_CardTFormsCalcer : ITFormsCalcer, IWithCamParams
    {
        public struct Config
        {
            /// <summary>
            /// Расположить карты лицевой или задней стороной к камере.
            /// </summary>
            public bool LookToCam { get; private set; }

            public Config(bool lookToCam = true)
            {
                LookToCam = lookToCam;
            }
        }

        private readonly Config _config;

        public CameraParams CamParams { get; set; }

        private Transform[] _cache = null;
        private int _cachedCardAmmount = 0;


        public ForSelect_CardTFormsCalcer(Config config)
        {
            _config = config;
        }


        public Transform[] Calc(int cardAmmount)
        {
            if (_cache == null || cardAmmount != _cachedCardAmmount)
            {
                _cachedCardAmmount = cardAmmount;
                _cache = ReCalc(cardAmmount);
            }

            var result = new Transform[cardAmmount];
            Array.Copy(_cache, result, cardAmmount);
            return result;
        }


        private Transform[] ReCalc(int cardAmmount)
        {
            CardsPlaneCalcer.FindMatricies(CamParams, out var cardsPlaneMMatrix, out var invCardsPlaneMMatrix);


            var result = new Transform[cardAmmount];
            var cardPoints = GenCardPointsInCardsPlane(cardAmmount);

            for (int i = 0; i < cardAmmount; i++)
            {
                var position = CalcWorldPosition(cardsPlaneMMatrix, cardPoints[i]);
                var rotation = CalcWorldRotation(cardsPlaneMMatrix);
                var scale = CalcWorldScale();
                result[i] = new Transform(position, rotation, scale);
            }

            return result;

        }


        protected Vector2[] GenCardPointsInCardsPlane(int pointsAmount)
        {
            //TODO: сделать нормальный алгоритм.
            return Enumerable.Range(0, pointsAmount)
                .Select(i =>
                {
                    return new Vector2(
                        -0.8f + (i % 5) * 0.4f,
                        0.8f - (int)(i / 5) * 0.8f);
                })
                .ToArray();
        }


        private Vector3 CalcWorldPosition(Matrix4x4 modelMatrix, Vector2 planePoint)
        {
            return Vector4.Transform(
                new Vector4(planePoint.X, planePoint.Y, 0, 1),
                Matrix4x4.Transpose(modelMatrix))
            .ToVector3();
        }


        private Quaternion CalcWorldRotation(Matrix4x4 modelMatrix)
        {
            var worldDir = modelMatrix.ApplyTo(LocalFrontDir()).ToVector3();
            var worldUp = modelMatrix.ApplyTo(new Vector4(Vector3.UnitY, 0)).ToVector3();

            return GeometryUtils.InitialRotation(worldDir, worldUp);
        }


        private Vector4 LocalFrontDir()
        {
            return _config.LookToCam ? Vector4.UnitZ : -Vector4.UnitZ;
        }


        private Vector3 CalcWorldScale()
        {
            var scaleFactor = 0.3f; // Пока захардкодим, но нужно определить нормальный механизм вычисления
            return new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }
}