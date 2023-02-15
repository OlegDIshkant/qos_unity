using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using static CommonTools.Math.NDCPointToPlaneSolver;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет расположения для карт игрока так, чтобы показать, что главный игрок (пользователь) выбирает у него карты для передачи себе.
    /// </summary>
    public class PlayerToMainPlayer_TransformMode_TFormsCalcer : IWithCamParamsAndPlayerTForm
    {
        private readonly CamOriented_SelectUnknown_CardTFormsCalcer _tFormCalcer; 

        public CameraParams CamParams
        {
            get => _tFormCalcer.CamParams;
            set => _tFormCalcer.CamParams = value;
        }


        public PlayerToMainPlayer_TransformMode_TFormsCalcer()
        {
            _tFormCalcer = new CamOriented_SelectUnknown_CardTFormsCalcer();
        }


        public void ChangePlayerTForm(Transform playerTForm)
        {
            // положение карт никак не зависит от игрока, который отдает карты
        }


        public Transform[] Calc(int cardAmmount) => _tFormCalcer.Calc(cardAmmount);
    }
}