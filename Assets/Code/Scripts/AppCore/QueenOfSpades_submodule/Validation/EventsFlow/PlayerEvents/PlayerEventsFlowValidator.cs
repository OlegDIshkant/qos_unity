using Qos.Domain.Entities;
using System.Collections.Generic;


namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// ¬алидирует поток событий, св€занных с игроком.
    /// </summary>
    internal class PlayerEventsFlowValidator : CompositeEventsFlowValidator
    {
        private readonly PlayerId _playerId;


        public PlayerEventsFlowValidator(PlayerId playerId)
        {
            _playerId = playerId;
            AddValidators(DefineValidators());
        }


        private List<IEventsFlowValidator> DefineValidators() =>
            new List<IEventsFlowValidator>()
            {
                new PlayerStartIdleEventFlowValidator(_playerId)
            };
    }
}