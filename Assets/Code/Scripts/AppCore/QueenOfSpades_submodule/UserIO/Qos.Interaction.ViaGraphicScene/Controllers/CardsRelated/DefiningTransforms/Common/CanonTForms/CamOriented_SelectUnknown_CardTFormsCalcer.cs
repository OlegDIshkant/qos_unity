using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет расположения карт для режима, когда главный игрок выбирает карты среди неизвестных ему карт другого игрока.
    /// </summary>
    internal class CamOriented_SelectUnknown_CardTFormsCalcer : ForSelect_CardTFormsCalcer, IWithCamParamsAndPlayerTForm
    {
        public CamOriented_SelectUnknown_CardTFormsCalcer() :
            base(new Config(lookToCam: false))
        {
        }


        public void ChangePlayerTForm(Transform playerTForm)
        {
            // считаем, что положение карт не зависит от положения игрока, карты которого выбирает главный игрок
        }
    }


}
