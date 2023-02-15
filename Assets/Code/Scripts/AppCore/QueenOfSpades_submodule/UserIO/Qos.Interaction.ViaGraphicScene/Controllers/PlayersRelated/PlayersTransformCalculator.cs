using System.Numerics;
using CommonTools.Math;
using CommonTools.Math.Geometry2D;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Алгоритм расчета канонических положений игроков.
    /// </summary>
    internal class PlayersTransformCalculator : ITFormsCalcer
    {
        private readonly IPlayFieldController _playField;


        public PlayersTransformCalculator(IPlayFieldController playField)
        {
            _playField = playField;
        }


        /// <summary>
        /// Пересчитать положения игроков.
        /// </summary>
        public Transform[] Calc(int objectsAmmount)
        {
            var result = new Transform[objectsAmmount];

            var fieldCenter = _playField.PlayFieldInfo.FloorCircleCenter;
            var relPositions = CalcPlayersRelativePositions(objectsAmmount);

            for (int i = 0; i < objectsAmmount; i++)
            {
                var position = CalcPosition(fieldCenter, relPositions[i]);
                var rotation = CalcRotation(position, fieldCenter);
                result[i] = new Transform(position, rotation);
            }

            return result;
        }


        private Vector2[] CalcPlayersRelativePositions(int objectsAmmount)
        {
            var radius = _playField.PlayFieldInfo.FloorCircleRadius;
            var screenPointsCalculator = new ElipsePointsCalculator(radius);
            return screenPointsCalculator.CalcPoints(objectsAmmount);
        }


        private Vector3 CalcPosition(Vector3 center, Vector2 offset2D)
        {
            var offset = new Vector3(offset2D.X, 0, offset2D.Y);
            return center + offset;
        }


        private Quaternion CalcRotation(Vector3 playerPosition, Vector3 centerPosition)
        {
            var direction = centerPosition - playerPosition;
            direction.Y = 0;
            return GeometryUtils.InitialRotation(direction);
        }



    }
}
