using System.Collections.Generic;

namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// ���������� ����� �������, ��������� � �������.
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