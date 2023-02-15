using CommonTools.Math;
using Qos.Domain.Entities;
using System.Collections.Generic;
using System.Numerics;
using static CommonTools.Math.NDCPointToPlaneSolver;

namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Выполняет вычисления для определения, как курсор взаимодействует с картами на эеране.
    /// </summary>
    public interface ICardsRaycaster
    {
        /// <summary>
        /// Определить, над какой из карт сейчас находится курсор.
        /// </summary>
        bool HasIntersection(IReadOnlyDictionary<CardId, Transform> transforms, Vector2 cursorPoint, CameraParams camParams, out CardId intersected);
    }
}
