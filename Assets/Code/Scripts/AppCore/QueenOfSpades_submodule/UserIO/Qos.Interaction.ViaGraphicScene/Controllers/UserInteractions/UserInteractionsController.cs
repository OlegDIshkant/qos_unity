using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{
    /// <summary>
    /// Контроллер взаимодействий пользователя с игровым миром.
    /// </summary>
    public class UserInteractionsController : EventController
    {
        private readonly ControllersQueue _controllersQueue;

        public UserInteractionsController(
            Contexts contexts,
            Func<IEvent> GetCurrentEvent,
            PlayerId mainPlayerId,
            Func<IDiscModeUi> DiscModeFatoryMethod,
            ICardTformsDefiner cardTformsDefiner) :
            base(contexts, GetCurrentEvent)
        {
            _controllersQueue = new ControllersQueue();
            _controllersQueue.AddWithoutTag(new MainPlayerAutoConfirmer(contexts, GetCurrentEvent));
            _controllersQueue.AddWithoutTag(new DiscModeUiController(contexts, mainPlayerId, DiscModeFatoryMethod, cardTformsDefiner, GetCurrentEvent));
            _controllersQueue.AddWithoutTag(new UserChoiceForCardTransferHandler(contexts, mainPlayerId, cardTformsDefiner, GetCurrentEvent));
        }


        public override void Update()
        {
            foreach (var controller in _controllersQueue.GetControllersInExecOrder())
            {
                controller.Update();
            }
        }
    }
}