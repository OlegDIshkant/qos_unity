using System.Collections.Generic;


namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// ������� ���������, ������������� � ���� ��� ������.
    /// </summary>
    public class EventsFlowValidator : CompositeEventsFlowValidator
    {
        public EventsFlowValidator()
        {
            AddValidatorSpawners(DefineValidatorSpawners());
        }


        protected List<IValidatorsSpawner> DefineValidatorSpawners() =>
            new List<IValidatorsSpawner>()
            {
                new PlayerEventsValidatorsSpawner(),
                new SimpleOnStartSpawner(() => new CardEventsFlowValidator())
            };

    }

}