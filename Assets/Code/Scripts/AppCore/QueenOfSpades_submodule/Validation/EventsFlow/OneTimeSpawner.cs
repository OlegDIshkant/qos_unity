using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections.Generic;


namespace Qos.Validation.EventsFlow
{
    public abstract class OneTimeSpawner : IValidatorsSpawner
    {
        private bool _didntSpawnYet = true;

        public IEnumerable<IEventsFlowValidator> TrySpawnValidators(IEvent @event)
        {
            if (_didntSpawnYet)
            {
                if (TrySpawn(@event, out var validators))
                {
                    _didntSpawnYet = false;
                    return validators;
                }
            }
            return null;
        }


        protected abstract bool TrySpawn(IEvent @event, out IEnumerable<IEventsFlowValidator> spawnedValidators);

    }
}