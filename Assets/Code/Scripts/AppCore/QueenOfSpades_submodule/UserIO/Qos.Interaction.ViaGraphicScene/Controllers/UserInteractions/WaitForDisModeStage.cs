using Qos.Domain.Entities;
using Qos.Domain.Events;
using System;
using System.Collections;
using static Qos.Domain.Events.PlayerChoosingCardsForDiscardEvent;


namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{
    internal class WaitForDisModeStage : InteractionStage
    {
        private readonly PlayerId _mainPlayerId;

        public BaseInteraction<InteractionArg> DiscModeInteraction { get; private set; }


        public WaitForDisModeStage(Func<IEvent> CurrentEvent, PlayerId mainPlayerId) : base(CurrentEvent)
        {
            _mainPlayerId = mainPlayerId;
        }


        protected override IEnumerator OnExecuting()
        {
            while (true)
            {
                var ev = CurrentEvent();
                if (IsDiscModeEvent(ev, out var discModeInteraction))
                {
                    DiscModeInteraction = discModeInteraction;
                    yield break;
                }
                else if (IsWontBeDiscModeEvent(ev))
                {
                    DiscModeInteraction = null;
                    yield break;
                }
                yield return null;
            }
        }


        private bool IsDiscModeEvent(IEvent @event, out BaseInteraction<InteractionArg> discModeInteraction)
        {
            if (@event is PlayerChoosingCardsForDiscardEvent pcdfdEvent && pcdfdEvent.PlayerId.Equals(_mainPlayerId))
            {
                discModeInteraction = pcdfdEvent.Interaction;
                return true;
            }
            discModeInteraction = null;
            return false;
        }


        private bool IsWontBeDiscModeEvent(IEvent @event) =>
            @event is PlayerStartIdleEvent pcdfdEvent && pcdfdEvent.PlayerId.Equals(_mainPlayerId);


    }
}