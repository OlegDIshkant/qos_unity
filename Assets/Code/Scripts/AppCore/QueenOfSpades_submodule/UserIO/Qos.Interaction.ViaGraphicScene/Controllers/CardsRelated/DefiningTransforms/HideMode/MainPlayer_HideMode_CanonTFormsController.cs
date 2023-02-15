using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ����������, ������������� ��������� ���� �������� ������ (������������), ����� ��� ���������� �� �� ���� (������),
    /// ����� ������� ����� ���� ������� ������ ��, ��� ������� ����.
    /// </summary>
    public class MainPlayer_HideMode_CanonTFormsController : CanonTFormsController<CardId>
    {
        public MainPlayer_HideMode_CanonTFormsController(
            Contexts contexts,
            PlayerId playerId,
            ICameraController cam,
            ICardsActionsProvider cardsActionsProvider) : 
            base(
                contexts, 
                new MainPlayerCanonCardsTFormsControllerImpl(
                    cam,
                    new MainPlayer_HideMode_TFormsCalcer(),
                    new WhenHideBeginCalcTFormsStartegy(playerId, cardsActionsProvider)),
                new CalcIndexInConstRndOrder(contexts.CardIdOrder))
        {
        }

    }
}