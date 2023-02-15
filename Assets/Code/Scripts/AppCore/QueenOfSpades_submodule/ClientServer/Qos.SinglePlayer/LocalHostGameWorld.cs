using Qos.AI;
using Qos.ClientServer;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.GameLogic.GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qos.SinglePlayer
{
    /// <summary>
    /// <see cref="IGameWorld"/>, который позволяет запускать игру с ботами локально.
    /// </summary>
    public class LocalHostGameWorld : BaseGameWorld
    {
        private PlayerId _mainPlayerId;
        private LocalProxyGameWorld _mainPlayerLPGW;
        private Dictionary<PlayerId, AiBotInfo> _aiInfo;
        private ProxyGameWorld _globalProxyGameWorld;

        public LocalHostGameWorld(
            IGameWorld actualGameWorld,
            PlayerId mainPlayerId,
            Dictionary<PlayerId, AbstractGameBot> aiBots)
        {

            var observerStreams = CreateObserverStreams(mainPlayerId, aiBots.Keys);
            _globalProxyGameWorld = new ProxyGameWorld(actualGameWorld, observerStreams, new EventCensor());
            InitLPGWs(mainPlayerId, aiBots, observerStreams);
        }


        private Dictionary<PlayerId, IEventStream> CreateObserverStreams(PlayerId mainPlayerId, IEnumerable<PlayerId> otherPlayerIds)
        {
            var result = otherPlayerIds.ToDictionary(
                id => id,
                id => new SimpleEventStream() as IEventStream);
            result.Add(mainPlayerId, new SimpleEventStream());

            return result;
        }


        private void InitLPGWs(
            PlayerId mainPlayerId,
            Dictionary<PlayerId, AbstractGameBot> aiBots,
            Dictionary<PlayerId, IEventStream> observerStreams)
        {
            _mainPlayerId = mainPlayerId;
            _mainPlayerLPGW = new LocalProxyGameWorld(observerStreams[_mainPlayerId]);

            _aiInfo = aiBots
                .ToDictionary(
                item => item.Key,
                item => new AiBotInfo()
                {
                    botBrains = item.Value,
                    botLPGW = new LocalProxyGameWorld(observerStreams[item.Key])
                });
        }


        public override IEnumerable<IEvent> Iterate()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("");
            }

            _globalProxyGameWorld.Iterate(); //Должно идти первым
            IterateAi();
            return GetEventsForMainPlayer();
        }


        private void IterateAi()
        {
            foreach (var item in _aiInfo)
            {
                var newEvents = item.Value.botLPGW.Iterate();
                item.Value.botBrains.Iterate(newEvents);
            }
        }


        private IEnumerable<IEvent> GetEventsForMainPlayer()
        {
            return _mainPlayerLPGW.Iterate();
        }



        private struct AiBotInfo
        {
            public AbstractGameBot botBrains;
            public LocalProxyGameWorld botLPGW;
        }
    }
}
