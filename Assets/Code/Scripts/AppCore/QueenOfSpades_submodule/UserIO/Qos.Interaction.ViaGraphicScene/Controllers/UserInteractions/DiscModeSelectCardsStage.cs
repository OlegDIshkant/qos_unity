using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using static Qos.Domain.Events.PlayerChoosingCardsForDiscardEvent;


namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{

    internal class DiscModeSelectCardsStage : BaseInteractionStage<InteractionArg>
    {
        private readonly Func<IDiscModeControl> _CreateDiscModeControl;
        private readonly ICardTformsDefiner _cardTformsDefiner;

        private IDiscModeControl _discModeControl;
        private InteractionArg _feedback;


        public DiscModeSelectCardsStage(
            ICardTformsDefiner cardTformsDefiner,
            Func<IDiscModeControl> CreateDiscModeControl,
            Func<IEvent> CurrentEvent, 
            BaseInteraction<InteractionArg> interaction) : 
            base(CurrentEvent, interaction)
        {
            _cardTformsDefiner = cardTformsDefiner;
            _CreateDiscModeControl = CreateDiscModeControl;
        }


        protected override void Update()
        {
            if (_cardTformsDefiner.CardsToDiscardByMainPlayer.HasChanged)
            {
                ActualizeConfirmButton();
            }
        }


        protected override InteractionArg Feedback() => _feedback;



        protected override void StartUi()
        {
            _discModeControl = _CreateDiscModeControl();
            _discModeControl.OnResultKnown += _uiControl_OnResultKnown;
            ActualizeConfirmButton();
        }


        protected override void EndUi()
        {
            _discModeControl.OnResultKnown -= _uiControl_OnResultKnown;
            _discModeControl.Dispose();
            _discModeControl = null;
        }


        private void _uiControl_OnResultKnown(DiscModeControlResult result)
        {
            switch (result)
            {
                case DiscModeControlResult.CONFIRM_CARDS:      _feedback = CardsSelectedFeedback(); break;
                case DiscModeControlResult.LEAVE_DISC_MODE:    _feedback = CancelDiscModeFeedback(); break;
                default:                                        throw new Exception();
            }
        }


        private InteractionArg CancelDiscModeFeedback() =>
            new InteractionArg()
            {
                selectedCardGroups = null,
                cancelDiscarding = true
            };



        private InteractionArg CardsSelectedFeedback()
        {
            var cardGroup = new CardGroup(_cardTformsDefiner.CardsToDiscardByMainPlayer.Items);

            return new InteractionArg()
            {
                cancelDiscarding = false,
                selectedCardGroups = new CardGroups() { cardGroup }
            };
        }


        private void ActualizeConfirmButton()
        {
            _discModeControl.ConfirmButtonEnabled = TwoCardsSelected();
        }


        private bool TwoCardsSelected() => 
            _cardTformsDefiner.CardsToDiscardByMainPlayer.Items.Count == 2;

    }
}
