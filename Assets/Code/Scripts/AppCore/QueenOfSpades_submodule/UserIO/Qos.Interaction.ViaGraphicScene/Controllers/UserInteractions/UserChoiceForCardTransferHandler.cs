using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms;
using System;
using System.Linq;
using ServerInteraction = Qos.Domain.Events.BaseInteraction<Qos.Domain.Events.PlayerChoosingCardsForTrasferEvent.InteractionArg>;


namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{
    /// <summary>
    ///  онтроллер, сообщающий в игровой мир (на "сервер"), какие карты главный игрок (пользователь) выбрал дл€ изъ€ти€ у другого игрока в режиме обмена картами.
    /// </summary>
    public class UserChoiceForCardTransferHandler : EventController
    {
        private readonly PlayerId _mainPlayerId;
        private readonly ICardTformsDefiner _cardTformsDefiner;

        private bool _waitingForServerToAsksForUserChoice = true;
        private ServerInteraction _interaction;


        public UserChoiceForCardTransferHandler(
            Contexts contexts, 
            PlayerId mainPlayerId,
            ICardTformsDefiner cardTformsDefiner,
            Func<IEvent> GetCurrentEvent) : 
            base(
                contexts, 
                GetCurrentEvent)
        {
            _mainPlayerId = mainPlayerId;
            _cardTformsDefiner = cardTformsDefiner;
        }


        public override void Update()
        {
            if (_waitingForServerToAsksForUserChoice)
            {
                WaitingForServerToAsksForUserChoice();
            }
            else
            {
                WaitingForUserToSelectCard();
            }
        }


        private void WaitingForServerToAsksForUserChoice()
        {
            if (CurrentEvent is PlayerChoosingCardsForTrasferEvent ev &&
                ev.Interaction != null &&
                ev.CardTakerId.Equals(_mainPlayerId))
            {
                StartWaitForUserToSelectCards(ev.Interaction);
            }
        }


        private void StartWaitForUserToSelectCards(ServerInteraction interaction)
        {
            _interaction = interaction;
            _waitingForServerToAsksForUserChoice = false;
        }


        private void StartWaitForServerToAsksForUserChoice()
        {
            _interaction = null;
            _waitingForServerToAsksForUserChoice = true;
        }


        private void WaitingForUserToSelectCard()
        {
            if (!ServerStillWaitsForUserChoice()) 
            {
                StartWaitForServerToAsksForUserChoice();
                return;
            }

            if (UserSelectedCard(out var card))
            {
                TellServerUserChoice(card);
                StartWaitForServerToAsksForUserChoice();
            }
        }


        private bool UserSelectedCard(out CardId card)
        {
            if (_cardTformsDefiner.CardsToTransferToMainPlayer.HasChanged && _cardTformsDefiner.CardsToTransferToMainPlayer.Items.Any())
            {
                card = _cardTformsDefiner.CardsToTransferToMainPlayer.Items[0];
                return true;
            }
            card = default;
            return false;
        }


        private void TellServerUserChoice(CardId card)
        {
            var arg = new PlayerChoosingCardsForTrasferEvent.InteractionArg(card.WrapInNewList());
            _interaction.Execute(arg);
        }


        private bool ServerStillWaitsForUserChoice() => _interaction.IsAlive;
    }
}