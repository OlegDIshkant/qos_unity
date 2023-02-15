using Qos.Domain.Entities;
using Qos.Domain.Events;
using System;
using System.Collections;
using static Qos.Domain.Events.PlayerStartIdleEvent;


namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{
    internal class WaitForPreDiscModeStage : InteractionStage
    {
        private readonly PlayerId _mainPlayerId;

        public BaseInteraction<InteractionArg> BeforeDiscModeInteraction { get; private set; } 


        public WaitForPreDiscModeStage(Func<IEvent> CurrentEvent, PlayerId mainPlayerId) : base(CurrentEvent)
        {
            _mainPlayerId = mainPlayerId;
        }


        protected override IEnumerator OnExecuting()
        {
            BaseInteraction<InteractionArg> _beforeDiscModeInteraction = null;
            while (!IsAllowedDiscModeEvent(CurrentEvent(), out _beforeDiscModeInteraction))
            {
                yield return null;
            }
            BeforeDiscModeInteraction = _beforeDiscModeInteraction;
        }


        private bool IsAllowedDiscModeEvent(IEvent @event, out BaseInteraction<InteractionArg> beforeDiscModeInteraction)
        {
            if (@event is PlayerStartIdleEvent psiEvent && psiEvent.Interaction != null && psiEvent.PlayerId.Equals(_mainPlayerId))
            {
                beforeDiscModeInteraction = psiEvent.Interaction;
                return true;
            }
            beforeDiscModeInteraction = null;
            return false;
        }
    }
}