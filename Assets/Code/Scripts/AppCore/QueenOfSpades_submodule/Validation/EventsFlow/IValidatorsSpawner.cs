using Qos.Domain.Events;
using System.Collections.Generic;


namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// Создает валидаторы событий.
    /// </summary>
    public interface IValidatorsSpawner 
    {
        IEnumerable<IEventsFlowValidator> TrySpawnValidators(IEvent @event);
    }
}