using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections.Generic;

namespace Qos.AI
{
    /// <summary>
    /// AI-Бот, который заменяет реального игрока.
    /// </summary>
    public abstract class AbstractGameBot
    {
        protected PlayerId PlayerId { get; private set; }


        public AbstractGameBot(PlayerId playerId)
        {
            PlayerId = playerId;
        }


        abstract public void Iterate(IEnumerable<IEvent> newEvents);
    }
}
