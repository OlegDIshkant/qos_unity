using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections;
using static Qos.Domain.Events.PlayerStartIdleEvent;
using DiscModeInteraction = Qos.Domain.Events.BaseInteraction<Qos.Domain.Events.PlayerChoosingCardsForDiscardEvent.InteractionArg>;


namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{
    public class DiscModeUiController : EventController
    {
        private readonly PlayerId _mainPlayerId;
        private readonly IEnumerator _mainRoutine;
        private readonly IDiscModeUi _discModeUi;
        private readonly ICardTformsDefiner _cardTformsDefiner;


        public DiscModeUiController(
            Contexts contexts,
            PlayerId mainPlayerId,
            Func<IDiscModeUi> DiscModeUiFactoryMethod,
            ICardTformsDefiner cardTformsDefiner,
            Func<IEvent> GetCurrentEvent) :
            base(contexts, GetCurrentEvent)
        {
            _mainPlayerId = mainPlayerId;
            _cardTformsDefiner = cardTformsDefiner;
            _discModeUi = DiscModeUiFactoryMethod();

            _mainRoutine = RoutineHelper.RecursiveIterator(MainRoutine());
        }


        public override void Update()
        {
            _mainRoutine.MoveNext();
        }


        private IEnumerator MainRoutine()
        {
            while (true)
            {
                var waitingForPreDiscMode = new WaitForPreDiscModeStage(GetCurrentEvent, _mainPlayerId);
                yield return waitingForPreDiscMode.Executing();

                if (waitingForPreDiscMode.BeforeDiscModeInteraction == null)
                {
                    yield return null; // "Фриза" не будет, потому что начнем новую итерацию в следущем кадре
                    continue;
                }

                DiscModeInteraction inDiscModeInteraction = null;
                yield return BeforeDiscMode(waitingForPreDiscMode.BeforeDiscModeInteraction, OnInteractionDefined: (i) => inDiscModeInteraction = i);

                while (inDiscModeInteraction != null)
                {
                    yield return DiscMode(inDiscModeInteraction, OnNextInteractionDefined: next => inDiscModeInteraction = next);
                }
            }
        }


        private IEvent GetCurrentEvent() => CurrentEvent;


        private IEnumerator BeforeDiscMode(BaseInteraction<InteractionArg> interaction, Action<DiscModeInteraction> OnInteractionDefined)
        {
            var beforeDiscModeStage = new BeforeDiscModeStage(_mainPlayerId, _discModeUi.EnablePreDiscModeControls, GetCurrentEvent, interaction);
            yield return beforeDiscModeStage.Executing();
            switch (beforeDiscModeStage.InteractionResult)
            {
                case BeforeDiscModeStage.Result.WONT_ENTER_DISC_MODE:
                case BeforeDiscModeStage.Result.HAS_ENTERED_DISC_MODE: OnInteractionDefined(beforeDiscModeStage.InDiscModeInteraction); break;
                default: throw new Exception();
            }
        }

        private IEnumerator DiscMode(DiscModeInteraction interaction, Action<DiscModeInteraction> OnNextInteractionDefined)
        {
            var inDiscMode = new DiscModeSelectCardsStage(_cardTformsDefiner, _discModeUi.EnableDiscModeControls, GetCurrentEvent, interaction);
            yield return inDiscMode.Executing();

            var waitForNextStep = new WaitForDisModeStage(GetCurrentEvent, _mainPlayerId);
            yield return waitForNextStep.Executing();
            OnNextInteractionDefined(waitForNextStep.DiscModeInteraction);
        }

    }




}
