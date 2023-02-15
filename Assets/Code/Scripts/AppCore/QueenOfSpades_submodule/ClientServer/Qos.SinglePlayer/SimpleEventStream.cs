using Qos.ClientServer;
using Qos.Domain.Events;
using System.Collections.Generic;

namespace Qos.SinglePlayer
{
    public class SimpleEventStream : IEventStream
    {
        private List<IEvent> _events = new List<IEvent>();

        public void PushEvents(IEnumerable<IEvent> events)
        {
            _events.AddRange(events);
        }

        public IEnumerable<IEvent> PullEvents()
        {
            var result = new List<IEvent>(_events);
            _events.Clear();
            return result;
        }
    }
}
