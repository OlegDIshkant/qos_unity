using CommonTools;
using Qos.Domain.Events;
using System.Collections.Generic;


namespace Qos.GameLogic.GameWorld
{
    public abstract class BaseGameWorld : DisposableClass, GameLogic.GameWorld.IGameWorld
    {
        public abstract IEnumerable<IEvent> Iterate();
    }
}
