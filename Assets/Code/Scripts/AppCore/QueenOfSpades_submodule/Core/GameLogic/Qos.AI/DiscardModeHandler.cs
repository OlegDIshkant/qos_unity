using Qos.Domain.Events;
using System;
using System.Threading.Tasks;
using static Qos.Domain.Events.PlayerStartIdleEvent;

namespace Qos.AI
{
    internal class DiscardModeHandler
    {

        public void OnCanRequestDiscardMode(BaseInteraction<InteractionArg> inetraction)
        {
            var _ = WaitAndRequestDiscardMode(inetraction);
        }


        private async Task WaitAndRequestDiscardMode(BaseInteraction<InteractionArg> interaction)
        {
            await Task.Delay(Helper.RandomDelay());
            if (interaction.IsAlive)
            {
                interaction.Execute(new InteractionArg(InteractionArg.Message.REQUEST_DISCARD_MODE));
            }
            else
            {
                Console.WriteLine("Хотел запросить режим 'сброса' карт, но уже поздно.");
            }
        }
    }
}
