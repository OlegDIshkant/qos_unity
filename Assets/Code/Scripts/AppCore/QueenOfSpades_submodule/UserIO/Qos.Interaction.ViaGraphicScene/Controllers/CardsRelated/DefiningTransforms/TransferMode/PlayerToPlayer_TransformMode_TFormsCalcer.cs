using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Вычисляет расположения для карт игрока так, чтобы показать, что другой игрок выбирает у него карты для передачи себе.
    /// </summary>
    public class PlayerToPlayer_TransformMode_TFormsCalcer : IWithTwoPlayerTForms
    {

        private readonly PlayerIdleTFormsCalcer _tFormCalcer; // пока сделаем так, чтобы это визуально ничем не отличалось от простого удержания игроком своих карт


        public PlayerToPlayer_TransformMode_TFormsCalcer(Transform firstPlayerInitialTransform)
        {
            _tFormCalcer = new PlayerIdleTFormsCalcer(firstPlayerInitialTransform, ParamsForCalcer());
        }


        private PlayerIdleTFormsCalcer.Params ParamsForCalcer() => new PlayerIdleTFormsCalcer.Params() { Scale = new Vector3(1.5f, 1.5f, 1.5f) };


        public void ChangeFirstPlayerTForm(Transform playerTForm)
        {
            _tFormCalcer.ChangePlayerTForm(playerTForm);
        }


        public void ChangeSecondPlayerTForm(Transform playerTForm)
        {
            // положение карт никак не зависит от второго игрока (который берет карты)
        }

        public Transform[] Calc(int cardAmmount) => _tFormCalcer.Calc(cardAmmount);
    }
}