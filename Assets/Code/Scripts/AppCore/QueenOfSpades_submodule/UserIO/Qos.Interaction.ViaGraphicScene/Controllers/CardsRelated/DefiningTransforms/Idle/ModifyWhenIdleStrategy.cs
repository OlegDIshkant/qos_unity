using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{

    /// <summary>
    /// Стратегия, которая просит модифицировать положения карт всё время, пока игрок, владеющий картами, ничем не занят.
    /// </summary>
    public class ModifyWhenIdleStrategy : SwitchableWhenToModifyStategy
    {
        private readonly PlayerId _playerId;
        private readonly ICardsActionsProvider _cardsActionsProvider;


        public ModifyWhenIdleStrategy(PlayerId playerId, ICardsActionsProvider cardsActionsProvider)
        {
            _playerId = playerId;
            _cardsActionsProvider = cardsActionsProvider;
        }


        protected override bool IfEnterModifiesAllowedState()
        {
            return AnyCardStartedIdle();
        }


        protected override bool IfExitModifiesAllowedState()
        {
            return AnyCardStopedIdling();
        }


        private bool AnyCardStartedIdle() =>
            _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId)
                .Any(i => i.Value.IsIdleNow(out var _));


        private bool AnyCardStopedIdling()
        {
            var allPrevActions = _cardsActionsProvider.PreviousActions.PlayerCardActions;

            if (allPrevActions?.ContainsKey(_playerId) ?? false)
            {
                var prevActions = allPrevActions[_playerId].Items;
                var changedActions = _cardsActionsProvider.GetAddedOrChangedPlayerActions(_playerId);

                foreach (var item in changedActions)
                {
                    if (prevActions.TryGetValue(item.Key, out var prevAction))
                    {
                        if (prevAction.IsIdleNow(out var _) && !item.Value.IsIdleNow(out var _))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


    }

}