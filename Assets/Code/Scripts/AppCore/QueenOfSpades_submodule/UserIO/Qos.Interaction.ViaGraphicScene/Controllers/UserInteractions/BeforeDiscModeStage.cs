using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections;
using InteractionArg = Qos.Domain.Events.PlayerStartIdleEvent.InteractionArg;


namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{
    internal class BeforeDiscModeStage : BaseInteractionStage<InteractionArg>
    {
        public enum Result { NONE, HAS_ENTERED_DISC_MODE, WONT_ENTER_DISC_MODE }

        public Result InteractionResult { get; private set; } = Result.NONE;
        public BaseInteraction<PlayerChoosingCardsForDiscardEvent.InteractionArg> InDiscModeInteraction { get; private set; }

        private readonly PlayerId _mainPlayerId;
        private readonly Func<IPreDiscModeControl> _CreatePreDiscModeControl;

        private IPreDiscModeControl _preDiscModeControl;
        private InteractionArg _feedback = null;


        public BeforeDiscModeStage(
            PlayerId mainPlayerId,
            Func<IPreDiscModeControl> CreatePreDiscModeControl,
            Func<IEvent> CurrentEvent,
            BaseInteraction<InteractionArg> interaction) : 
            base(CurrentEvent, interaction)
        {
            _mainPlayerId = mainPlayerId;
            _CreatePreDiscModeControl = CreatePreDiscModeControl;
        }


        protected override InteractionArg Feedback() => _feedback;


        protected override void StartUi()
        {
            _preDiscModeControl = _CreatePreDiscModeControl();
            _preDiscModeControl.OnResultKnown += _uiControl_OnResultKnown;
        }


        protected override void EndUi()
        {
            _preDiscModeControl.OnResultKnown -= _uiControl_OnResultKnown;
            _preDiscModeControl.Dispose();
            _preDiscModeControl = null;
        }

        private void _uiControl_OnResultKnown(PreDiscModeControlResult result)
        {
            switch (result)
            {
                case PreDiscModeControlResult.GO_TO_DISC_MODE: _feedback = GoToDiscModeFeedback(); break;
                case PreDiscModeControlResult.WONT_GO_DISC_MODE: _feedback = DontGoToDiscModeFeedback(); break;
                default: throw new Exception();
            }
        }


        private InteractionArg GoToDiscModeFeedback() => new InteractionArg(InteractionArg.Message.REQUEST_DISCARD_MODE);


        private InteractionArg DontGoToDiscModeFeedback() => new InteractionArg(InteractionArg.Message.WONT_REQUEST_DISCARD_MODE);


        protected override IEnumerator AfterInteractionCompleted()
        {
            var waitForDiscMode = new WaitForDisModeStage(CurrentEvent, _mainPlayerId);
            yield return waitForDiscMode.Executing();

            InDiscModeInteraction = waitForDiscMode.DiscModeInteraction;
            InteractionResult = (InDiscModeInteraction != null) ? Result.HAS_ENTERED_DISC_MODE : Result.WONT_ENTER_DISC_MODE;
        }


    }
}
