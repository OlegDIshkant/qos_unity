using Qos.Domain.Events;
using System;
using System.Collections;

namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{

    internal abstract class InteractionStage<T> : InteractionStage
    {
        private readonly BaseInteraction<T> _interaction;


        protected InteractionStage(Func<IEvent> CurrentEvent, BaseInteraction<T> interaction) : base(CurrentEvent)
        {
            _interaction = interaction;
        }

        protected override sealed IEnumerator OnExecuting()
        {
            yield return OnExecuting(_interaction);
        }


        protected abstract IEnumerator OnExecuting(BaseInteraction<T> interaction);

    }



    internal abstract class InteractionStage
    {
        protected Func<IEvent> CurrentEvent { get; private set; }

        private bool _wasLaunched = false;


        public InteractionStage(Func<IEvent> CurrentEvent)
        {
            this.CurrentEvent = CurrentEvent;
        }


        public IEnumerator Executing()
        {
            MakeSureLaunchedOnce();
            yield return OnExecuting();
        }


        protected abstract IEnumerator OnExecuting();


        private void MakeSureLaunchedOnce()
        {
            if (_wasLaunched)
            {
                throw new InvalidOperationException();
            }
            _wasLaunched = true;
        }

    }
}
