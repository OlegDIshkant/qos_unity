using Qos.Domain.Events;
using System;
using System.Collections.Generic;


namespace Qos
{
    /// <summary>
    /// Describes a type responsible for providing I/O of the game to a user.
    /// </summary>
    public interface IInteractor : IDisposable
    {
        bool ExitRequested { get; }
        void DepictEvents(ICollection<IEvent> @event);
    }
}
