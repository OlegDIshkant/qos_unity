using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static CommonTools.Math.NDCPointToPlaneSolver;

namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Самая простая и примитивная реализация <see cref="ICardsRaycaster"/>.
    /// </summary>
    internal class PrimitiveCardsRaycaster : ICardsRaycaster
    {

        public bool HasIntersection(IReadOnlyDictionary<CardId, Transform> transforms, Vector2 cursorPoint, CameraParams camParams, out CardId intersected)
        {
            // Просто ищем карту, к проекции которой сейчас ближе всего курсор.
            var (nearest, dist) = transforms
                .FindIndexWithMin(item => 
                { 
                    var screenPoint = item.Value.Position.WorldToNDC(camParams).ToVector2(); 
                    return Vector2.Distance(screenPoint, cursorPoint); 
                });


            // Если растояние от курсора до ближайшей карты (до её проекции на экран) достаточно мало, считаем, что курсор над ней.
            if (dist < 0.2f)
            {
                intersected = transforms.Skip(nearest).First().Key;
                return true;
            }

            intersected = default;
            return false;
        }

    }
}
