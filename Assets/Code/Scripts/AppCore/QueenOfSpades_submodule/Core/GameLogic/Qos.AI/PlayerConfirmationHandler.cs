using System;
using System.Threading.Tasks;

namespace Qos.AI
{
    internal class PlayerConfirmationHandler
    {
        public bool IsBusy { get; private set; } = false;

        public void Start(Action ToConfirmPlayer)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException();
            }
            IsBusy = true;

            WaitAndConfirmThisPlayer(ToConfirmPlayer)
                .GetAwaiter()
                .OnCompleted(() => IsBusy = false);
        }

        private async Task WaitAndConfirmThisPlayer(Action ToConfirmPlayer)
        {
            await Task.Delay(Helper.RandomDelay());
            ToConfirmPlayer();
        }
    }
}
