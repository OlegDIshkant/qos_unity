using Qos.Domain.Entities;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Стратегия, требующая считать положения для карт в тот момент, когда очередная из них создается.
    /// </summary>
    public class CalcThenCardsStartToAppear : CardsRelatedBeginCalcTFormsStrategy
    {
        public CalcThenCardsStartToAppear(ICardsActionsProvider cardsActionsProvider) : base(cardsActionsProvider)
        {
        }


        public override bool IsBeginCalcTFormsEvent(out IEnumerable<CardId> objects)
        {
            objects = CardsThatHaveBeganToAppear();
            return objects.Any();
        }


        public override bool IsStopCalcTFormsEvent(out IEnumerable<CardId> objects)
        {
            objects = null;
            return false;
        }


        private IEnumerable<CardId> CardsThatHaveBeganToAppear()
        {
            return CardsActionsProvider.NonPlayerCardActions.AddedOrChanged
                .Where(i => i.Value.IsCreatingNow())
                .Select(i => i.Key);

        }
    }
}