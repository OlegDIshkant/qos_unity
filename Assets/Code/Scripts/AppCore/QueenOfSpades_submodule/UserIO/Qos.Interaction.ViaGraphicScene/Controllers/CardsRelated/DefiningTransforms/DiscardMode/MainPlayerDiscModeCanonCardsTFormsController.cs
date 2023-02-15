using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, определяющий канонические положения карт главного игрока в режиме выбора карт для сброса.
    /// </summary>
    public class MainPlayerDiscModeCanonCardsTFormsController : CanonTFormsController<CardId>
    {
        public MainPlayerDiscModeCanonCardsTFormsController(
            Contexts contexts,
            PlayerId playerId,
            ICameraController cameraController,
            ICardsActionsProvider cardsActionsProvider) :
            base(
                contexts,
                new MainPlayerCanonCardsTFormsControllerImpl(
                    cameraController,
                    new CamOriented_SelectKnown_CardTFormsCalcer(),
                    new WhenDiscModeBeginCalcTFormsStartegy(playerId, cardsActionsProvider)),
                new CalcIndexInConstRndOrder(contexts.CardIdOrder))
        {
        }
    }
}