

namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{

    /// <summary>
    /// Вычисляет расположения карт для режима, когда главный игрок (пользователь) выбирает карты среди известных ему.
    /// </summary>
    internal class CamOriented_SelectKnown_CardTFormsCalcer : ForSelect_CardTFormsCalcer
    {
        public CamOriented_SelectKnown_CardTFormsCalcer() :
            base(new Config(lookToCam: true))
        {
        }
    }
}