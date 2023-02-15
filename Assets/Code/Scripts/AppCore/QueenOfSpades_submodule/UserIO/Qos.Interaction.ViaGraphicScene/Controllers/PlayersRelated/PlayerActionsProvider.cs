using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Контроллер, предоставляющий информацию о том, что в данный момент времени должно происходить с тем или иным игроком,
    /// а также, что должно было происходить с тем или иным игроком в предыдущий момент времени.
    /// </summary>
    public class PlayerActionsProvider : EventController, IPlayerActionsProvider
    {
        private DefiningPlayersActions _currentActions;
        private DefiningPlayersActions _prevActions;

        private IEvent _currentEvent;
        private IEvent _prevEvent;

        public IPlayerActionsDefiner PreviousActions => _prevActions;
        public DictionaryData<PlayerId, PlayerAction> DictionaryOutput => _currentActions.DictionaryOutput;


        public PlayerActionsProvider(Contexts contexts, Func<IEvent> GetCurrentEvent) :
            base(contexts, GetCurrentEvent)
        {
            _currentActions = new DefiningPlayersActions(contexts, this.GetCurrentEvent);
            _prevActions = new DefiningPlayersActions(contexts, this.GetPrevEvent);
        }


        private IEvent GetCurrentEvent() => _currentEvent;


        private IEvent GetPrevEvent() => _prevEvent;


        public override void Update()
        {
            _prevEvent = _currentEvent;
            _currentEvent = CurrentEvent;

            _currentActions.Update();
            _prevActions.Update();
        }

    }
}