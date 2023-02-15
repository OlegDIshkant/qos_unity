using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using static CommonTools.Math.NDCPointToPlaneSolver;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет расположения для карт главного игрока так, чтобы показать, что другой игрок выбирает у него карты для передачи себе.
    /// </summary>
    public class MainPlayerToPlayer_TransformMode_TFormsCalcer : IWithCamParamsAndPlayerTForm
    {
        private readonly MainPlayerIdleCardTFormsCalcer _tFormCalcer; // пока сделаем так, чтобы это визуально ничем не отличалось от простого удержания главным игроком своих карт

        public CameraParams CamParams
        {
            get => _tFormCalcer.CamParams;
            set => _tFormCalcer.CamParams = value;
        }


        public MainPlayerToPlayer_TransformMode_TFormsCalcer()
        {
            _tFormCalcer = new MainPlayerIdleCardTFormsCalcer();
        }


        public void ChangePlayerTForm(Transform playerTForm)
        {
            // положение карт никак не зависит от второго игрока (который берет карты)
        }


        public Transform[] Calc(int cardAmmount) => _tFormCalcer.Calc(cardAmmount);
    }
}