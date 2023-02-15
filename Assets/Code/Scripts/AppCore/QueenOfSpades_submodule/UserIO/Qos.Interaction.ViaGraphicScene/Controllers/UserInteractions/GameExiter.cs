using Qos.Domain.Events;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers
{
    /// <summary>
    /// Контроллер, позволяющий выйти из игры.
    /// </summary>
    public class GameExiter : EventController
    {
        private readonly Action _OnExitRequested;


        private bool _exitWasRequested = false;


        public GameExiter(
            Contexts contexts, 
            Func<IEvent> GetCurrentEvent,
            Action OnExitRequested) :
            base(
                contexts, 
                GetCurrentEvent)
        {
            _OnExitRequested = OnExitRequested;
        }


        public override void Update()
        {
            if (!_exitWasRequested)
            {
                UpdateAction();
            }
        }


        private void UpdateAction()
        {
            if (ExitNow())
            {
                RequestExit();
            }
        }


        public bool ExitNow()
        {
            return CurrentEvent.IsEndOfContiniousEvent<MatchFinsihedEvent>(out var _);
        }


        private void RequestExit()
        {
            _exitWasRequested = true;
            _OnExitRequested();
        }
    }
}