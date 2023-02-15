using Qos.Domain.Events;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// Валидирует поток игровых событий.
    /// </summary>
    public interface IEventsFlowValidator 
    {
        /// <summary>
        /// Каждое новое игровое событие должно быть передано в порядке появления 
        /// в данный метод для проверки на наличие отклонений от ожидаемой "логики".
        /// </summary>
        /// <returns>
        /// Если восращаемый список проблем пуст или не существует, значит проблем не обнаружено.
        /// </returns>
        IEnumerable<Problem> CheckNextEvent(IEvent @event);
    }


    /// <summary>
    /// Описание какой-либо проблемы.
    /// </summary>
    public struct Problem
    {
        public string Message { get; private set; }

        public Problem(string msg)
        {
            Message = msg;
        }
    }
}