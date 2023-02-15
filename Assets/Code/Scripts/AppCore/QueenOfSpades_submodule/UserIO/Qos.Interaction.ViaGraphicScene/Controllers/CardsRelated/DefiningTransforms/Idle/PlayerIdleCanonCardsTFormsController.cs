using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.CanonCardTransforms;
using System.Numerics;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ����������, ������������ ������������ ��������� ���� ������, ����� ��� ����� ��������� �� �����.
    /// </summary>
    public class PlayerIdleCanonCardsTFormsController : CanonTFormsController<CardId>
    {
        public PlayerIdleCanonCardsTFormsController(
            Contexts contexts,
            PlayerId playerId,
            IPlayersTFormsDefiner playerTFormsDefiner,
            ICardsActionsProvider cardsActionsProvider) :
            base(
                contexts,
                new PlayerCanonCardsTFormsControllerImpl(
                    playerId,
                    playerTFormsDefiner,
                    new PlayerIdleTFormsCalcer(new Transform(Vector3.Zero)),
                    new WhenIdleBeginCalcTFormsStartegy(playerId, cardsActionsProvider)),
                new CalcIndexInConstRndOrder(contexts.CardIdOrder))
        {
        }

    }
}