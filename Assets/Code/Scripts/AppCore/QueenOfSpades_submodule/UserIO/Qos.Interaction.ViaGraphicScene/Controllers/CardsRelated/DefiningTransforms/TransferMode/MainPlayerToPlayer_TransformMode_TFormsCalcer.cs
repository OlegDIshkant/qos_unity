using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using static CommonTools.Math.NDCPointToPlaneSolver;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ��������� ������������ ��� ���� �������� ������ ���, ����� ��������, ��� ������ ����� �������� � ���� ����� ��� �������� ����.
    /// </summary>
    public class MainPlayerToPlayer_TransformMode_TFormsCalcer : IWithCamParamsAndPlayerTForm
    {
        private readonly MainPlayerIdleCardTFormsCalcer _tFormCalcer; // ���� ������� ���, ����� ��� ��������� ����� �� ���������� �� �������� ��������� ������� ������� ����� ����

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
            // ��������� ���� ����� �� ������� �� ������� ������ (������� ����� �����)
        }


        public Transform[] Calc(int cardAmmount) => _tFormCalcer.Calc(cardAmmount);
    }
}