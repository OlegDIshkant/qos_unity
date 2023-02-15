using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ��������� ������������ ��� ���� ������ ���, ����� ��������, ��� ������ ����� �������� � ���� ����� ��� �������� ����.
    /// </summary>
    public class PlayerToPlayer_TransformMode_TFormsCalcer : IWithTwoPlayerTForms
    {

        private readonly PlayerIdleTFormsCalcer _tFormCalcer; // ���� ������� ���, ����� ��� ��������� ����� �� ���������� �� �������� ��������� ������� ����� ����


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
            // ��������� ���� ����� �� ������� �� ������� ������ (������� ����� �����)
        }

        public Transform[] Calc(int cardAmmount) => _tFormCalcer.Calc(cardAmmount);
    }
}