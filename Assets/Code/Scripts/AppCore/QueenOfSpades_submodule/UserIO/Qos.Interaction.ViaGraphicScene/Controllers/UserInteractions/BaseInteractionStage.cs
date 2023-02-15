using Qos.Domain.Events;
using System;
using System.Collections;


namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{
    internal abstract class BaseInteractionStage<T> : InteractionStage<T>
        where T : class
    {

        public BaseInteractionStage(Func<IEvent> CurrentEvent, BaseInteraction<T> interaction) : base(CurrentEvent, interaction)
        {
        }


        protected override sealed IEnumerator OnExecuting(BaseInteraction<T> interaction)
        {

            T feedback = default;
            StartUi();
            while (NothingChanges())
            {
                Update();
                yield return null;
                feedback = Feedback();
            }
            EndUi();
            CompleteInteraction(interaction, feedback);

            bool NothingChanges() => interaction.IsAlive && feedback == null;

            yield return AfterInteractionCompleted();
        }

        protected abstract T Feedback();

        protected virtual void Update() { }


        protected abstract void StartUi();


        protected abstract void EndUi();


        private void CompleteInteraction(BaseInteraction<T> interaction, T feedback)
        {
            if (interaction.IsAlive && feedback != null)
            {
                interaction.Execute(feedback);
            }
        }


        protected virtual IEnumerator AfterInteractionCompleted()
        {
            yield break;
        }
    }
}
