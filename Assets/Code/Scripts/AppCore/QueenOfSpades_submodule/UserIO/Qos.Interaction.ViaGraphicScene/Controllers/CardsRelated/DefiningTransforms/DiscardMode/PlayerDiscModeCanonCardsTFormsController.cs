using Qos.Domain.Entities;
using System.Numerics;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    ///  онтроллер, определ€ющий канонические положени€ карт игрока в режиме выбора карт дл€ сброса.
    /// </summary>
    public class PlayerDiscModeCanonCardsTFormsController : CanonTFormsController<CardId>
    {
        public PlayerDiscModeCanonCardsTFormsController(
            Contexts contexts,
            IPlayersTFormsDefiner playersTFormsDefiner,
            ICardsActionsProvider cardsActionsProvider,
            PlayerId playerId) :
            base(
                contexts,
                new PlayerCanonCardsTFormsControllerImpl(
                    playerId,
                    playersTFormsDefiner,
                    new PlayerIdleTFormsCalcer(new Transform(Vector3.Zero)),
                    new WhenDiscModeBeginCalcTFormsStartegy(playerId, cardsActionsProvider)),
                new CalcIndexInConstRndOrder(contexts.CardIdOrder))
        {
        }
    }
}