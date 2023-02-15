using CommonTools;
using Qos.Domain.Events;
using System;
using System.Collections.Generic;


namespace Qos.Validation.EventsFlow
{
    public abstract class OnStartSpawner : OneTimeSpawner
    {
        protected override bool TrySpawn(IEvent @event, out IEnumerable<IEventsFlowValidator> spawnedValidators)
        {
            spawnedValidators = Validators();
            return true;
        }


        protected abstract IEnumerable<IEventsFlowValidator> Validators();
    }




    public class SimpleOnStartSpawner : OnStartSpawner
    {
        private readonly Func<IEnumerable<IEventsFlowValidator>> _CreateValidators;


        public SimpleOnStartSpawner(Func<IEventsFlowValidator> CreateValidator)
        {
            _CreateValidators = () => CreateValidator().WrapInNewList();
        }


        public SimpleOnStartSpawner(Func<IEnumerable<IEventsFlowValidator>> CreateValidators)
        {
            _CreateValidators = CreateValidators;
        }


        protected override IEnumerable<IEventsFlowValidator> Validators() => _CreateValidators();
    }
}