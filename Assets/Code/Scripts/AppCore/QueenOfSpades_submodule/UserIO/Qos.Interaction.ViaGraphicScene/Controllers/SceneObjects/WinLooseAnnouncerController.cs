using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects
{
    /// <summary>
    /// Контроллер, дающий объявления о том, выиграл главный игрок (пользователь) или проиграл.
    /// </summary>
    public class WinLooseAnnouncerController : EventController
    {
        private readonly Func<IWinLooseAnnouncer> _AnnouncerFactoryMethod;

        private IWinLooseAnnouncer _winLooseAnnouncer;

        public WinLooseAnnouncerController(
            Contexts contexts, 
            Func<IEvent> GetCurrentEvent,
            Func<IWinLooseAnnouncer> AnnouncerFactoryMethod) 
            : base(
                  contexts, 
                  GetCurrentEvent)
        {
            _AnnouncerFactoryMethod = AnnouncerFactoryMethod;
        }


        public override void Update()
        {
            if (ToLaunchAnnouncement(out var announceWin))
            {
                if (announceWin)
                {
                    GetAnnouncer().LaunchWinAnnouncement();
                }
                else
                {
                    GetAnnouncer().LaunchLooseAnnouncement();
                }
            }
        }


        private bool ToLaunchAnnouncement(out bool announceWin)
        {

            if (CurrentEvent is PlayerNotLostMatchEvent pnlmEvent &&
                pnlmEvent.PlayerId.Equals(Contexts.PlayersInfo.mainPLayerId))
            {
                announceWin = true;
                return true;
            }
            else if (CurrentEvent is PlayerLostMatchEvent plmEvent &&
                plmEvent.PlayerId.Equals(Contexts.PlayersInfo.mainPLayerId))
            {
                announceWin = false;
                return true;
            }

            announceWin = default;
            return false;
        }


        private IWinLooseAnnouncer GetAnnouncer()
        {
            if (_winLooseAnnouncer == null)
            {
                _winLooseAnnouncer = _AnnouncerFactoryMethod();
            }
            return _winLooseAnnouncer;
        }
    }
}