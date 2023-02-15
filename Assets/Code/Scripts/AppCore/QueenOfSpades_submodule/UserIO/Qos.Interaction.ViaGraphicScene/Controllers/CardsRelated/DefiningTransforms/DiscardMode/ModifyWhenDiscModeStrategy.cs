using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Linq;

namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{

    /// <summary>
    /// Стратегия, которая просит модифицировать положения карт всё время, пока игрок, владеющий картами, находится в режиме выбора карт для сброса.
    /// </summary>
    public class ModifyWhenDiscModeStrategy : SwitchableWhenToModifyStategy
    {
        private readonly PlayerId _playerId;
        private readonly ICardsActionsProvider _cardsActionsProvider;


        public ModifyWhenDiscModeStrategy(PlayerId playerId, ICardsActionsProvider cardsActionsProvider)
        {
            _playerId = playerId;
            _cardsActionsProvider = cardsActionsProvider;
        }


        protected override bool IfEnterModifiesAllowedState() => DiscModeStarted();


        private bool DiscModeStarted() =>
            _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId).Any(i => i.Value.IsInDiscMode(out var _));


        protected override bool IfExitModifiesAllowedState() => CardChoosedForDiscarding();


        private bool CardChoosedForDiscarding() =>
            _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId).Any(i => i.Value.IsSelectedForDiscard(out var _));
    }
}