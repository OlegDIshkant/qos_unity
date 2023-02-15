using Qos.Domain.Events;
using System.Collections.Generic;

namespace Qos.ClientServer
{
    public interface IEventStream
    {
        void PushEvents(IEnumerable<IEvent> events);


        /// <summary>
        /// Получить все накопленные к этому моменту события.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEvent> PullEvents();
    }
}
