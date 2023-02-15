using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, расчитывающий канонические положения карт игрока, когда тот отодвигает их от себя (прячет),
    /// чтобы выбрать среди карт другого игрока те, что заберет себе.
    /// </summary>
    public class Player_HideMode_CanonTFormsController : CanonTFormsController<CardId>
    {
        public Player_HideMode_CanonTFormsController(
            Contexts contexts,
            PlayerId playerId,
            IPlayersTFormsDefiner playerTFormsDefiner,
            ICardsActionsProvider cardsActionsProvider) :
            base(
                contexts,
                new PlayerCanonCardsTFormsControllerImpl(
                    playerId,
                    playerTFormsDefiner,
                    new Player_HideMode_TFormsCalcer(Transform.Default),
                    new WhenHideBeginCalcTFormsStartegy(playerId, cardsActionsProvider)),
                new CalcIndexInConstRndOrder(contexts.CardIdOrder))
        {
        }

    }
}