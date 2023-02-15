using Qos.Domain.Events;
using Qos.GameLogic.GameWorld;
using System;
using System.Collections.Generic;


namespace Qos.ClientServer
{
    /// <summary>
    /// Локальный прокси игрового мира, который получает игровые события от "центра" через "линию связи".
    /// </summary>
    public class LocalProxyGameWorld : BaseGameWorld
    {
        private readonly IEventStream _eventStream;


        public LocalProxyGameWorld(IEventStream eventStream)
        {
            _eventStream = eventStream;
        }


        public override IEnumerable<IEvent> Iterate()
        {
            if (IsDisposed) throw new ObjectDisposedException("");
            return _eventStream.PullEvents();
        }



    }
}
