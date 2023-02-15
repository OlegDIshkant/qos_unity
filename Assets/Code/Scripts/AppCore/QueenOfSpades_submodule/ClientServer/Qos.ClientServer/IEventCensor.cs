using Qos.Domain.Entities;
using Qos.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qos.ClientServer
{
    /// <summary>
    /// Сензурирует игровые события, удаляя из них ту часть информации,
    /// которую не положено видеть конкретному пользователю.
    /// </summary>
    public interface IEventCensor
    {
        IEnumerable<IEvent> CensorEvents(PlayerId observer, IEnumerable<IEvent> originalEvents);
    }
}
