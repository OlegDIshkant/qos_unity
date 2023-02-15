using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections.Generic;


namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// Спавнит те валидаторы событий, связанных с игроками, которые зависят от данных, определяемых динамически во время работы. 
    /// </summary>
    public class PlayerEventsValidatorsSpawner : OneTimeSpawner
    {
        protected override bool TrySpawn(IEvent @event, out IEnumerable<IEventsFlowValidator> spawnedValidators)
        {
            if (@event is PlayersExpectedEvent peEvent)
            {
                spawnedValidators = Spawn(peEvent.PlayerIds);
                return true;
            }
            spawnedValidators = null;
            return false;
        }


        private IEnumerable<IEventsFlowValidator> Spawn(IEnumerable<PlayerId> playerIds)
        {
            var validators = new List<IEventsFlowValidator>();

            foreach (var player in playerIds)
            {
                validators.Add(new PlayerEventsFlowValidator(player));
            }
            validators.Add(new PlayersLeaveMatchEventsValidator());

            return validators;
        }
    }
}