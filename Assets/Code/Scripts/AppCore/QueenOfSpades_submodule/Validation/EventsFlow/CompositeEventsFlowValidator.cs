using CommonTools;
using Qos.Domain.Events;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// ¬алидатор, интегрирующий в себ€ другие валидаторы.
    /// </summary>
    public abstract class CompositeEventsFlowValidator : IEventsFlowValidator
    {
        private readonly List<IEventsFlowValidator> _validators;
        private readonly List<IValidatorsSpawner> _spawners;

        private List<Problem> _problems = new List<Problem>();


        public CompositeEventsFlowValidator()
        {
            _validators = new List<IEventsFlowValidator>();
            _spawners = new List<IValidatorsSpawner>();
        }


        protected void AddValidators(IEnumerable<IEventsFlowValidator> validators)
        {
            _validators.AddRange(validators);
        }


        protected void AddValidatorSpawners(IEnumerable<IValidatorsSpawner> spawners)
        {
            _spawners.AddRange(spawners);
        }


        public IEnumerable<Problem> CheckNextEvent(IEvent @event)
        {
            TrySpawnNewValidators(@event);
            return Validate(@event);
        }


        private void TrySpawnNewValidators(IEvent @event)
        {
            foreach (var spawner in _spawners)
            {
                var newValidators = spawner.TrySpawnValidators(@event);
                if (newValidators != null && newValidators.Any())
                {
                    _validators.AddRange(newValidators);
                    LogSpawnedValidators(newValidators);
                }
            }
        }


        private void LogSpawnedValidators(IEnumerable<IEventsFlowValidator> validators)
        {
            foreach (var v in validators)
            {
                Logger.Verbose($"Ќовый валидатор создан: '{v}'.");
            }
        }


        private IEnumerable<Problem> Validate(IEvent @event)
        {
            _problems.Clear();
            foreach (var validator in _validators)
            {
                var foundProblems = validator.CheckNextEvent(@event);
                if (foundProblems != null && foundProblems.Any())
                {
                    _problems.AddRange(foundProblems);
                }
            }
            return _problems;
        }
    }
}