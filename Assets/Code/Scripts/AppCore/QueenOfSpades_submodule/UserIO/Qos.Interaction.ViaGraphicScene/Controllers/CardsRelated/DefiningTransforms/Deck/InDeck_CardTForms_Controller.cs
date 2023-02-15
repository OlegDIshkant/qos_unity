using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, считающий положения карт в ситуациях, когда они находятся в стартовой колоде.
    /// </summary>
    public class InDeck_CardTForms_Controller : CanonTFormsController<CardId>
    {
        public InDeck_CardTForms_Controller(
            Contexts contexts,
            IDeckController deckController,
            ICardsActionsProvider cardsActionsProvider) : 
            base(
                contexts, 
                new CanonTFormsControllerImpl<CardId> (
                    new CalcThenCardsStartToAppear(cardsActionsProvider),
                    new InDeck_cardTForms_Calcer(deckController)))
        {
        }

    }
}
