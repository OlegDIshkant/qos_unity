using System.Collections.Generic;

namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// ¬алидирует поток событий, св€занных с картами.
    /// </summary>
    public class CardEventsFlowValidator : CompositeEventsFlowValidator
    {
        public CardEventsFlowValidator()
        {
            var validators = new List<IEventsFlowValidator>()
            {
                new CardOwnershipValidator()
            };
            AddValidators(validators);
        }

    }
}