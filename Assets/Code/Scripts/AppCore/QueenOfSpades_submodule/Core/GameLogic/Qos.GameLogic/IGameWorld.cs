using Qos.Domain.Events;
using System;
using System.Collections.Generic;


namespace Qos.GameLogic.GameWorld
{
    /// <summary>
    /// It's a black box which hides all main game logic from rest of the system.
    /// </summary>
    public interface IGameWorld : IDisposable
    {
        /// <summary>
        /// You can know what happened in the game by iterating the world and extracting game events.
        /// </summary>
        IEnumerable<IEvent> Iterate();
    }
}
