using Qos.Domain.Events;
using System;
using System.Linq;

namespace Qos.Interaction.ViaGraphicScene.Controllers.UserInteractions
{
    public class MainPlayerAutoConfirmer : EventController
    {
        public MainPlayerAutoConfirmer(Contexts contexts, Func<IEvent> GetCurrentEvent) : base(contexts, GetCurrentEvent)
        {
        }

        public override void Update()
        {
            if (CurrentEvent is PlayersExpectedEvent peEvent)
            {
                ConfirmMainPlayer();
            }

            void ConfirmMainPlayer()
            {
                peEvent.Confirmers.Single().Value.Execute();
            }
        }


    }
}
