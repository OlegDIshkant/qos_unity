using CGA;
using Qos.GameLogic.GameWorld;
using System;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene;
using Qos.SinglePlayer;
using Qos.AI;
using Qos.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Qos.GameLogic.GameWorld.Stages;


namespace QosGameApp
{
    internal class QosGameFactory : IAppCoreFactory
    {
        private readonly Func<IGraphicSceneFactory> _CreateGraphicSceneFactory;

        public QosGameFactory(Func<IGraphicSceneFactory> CreateGraphicSceneFactory)
        {
            _CreateGraphicSceneFactory = CreateGraphicSceneFactory;
        }


        public IAppCore GenerateAppCore()
        {
            NewPlayerIds(out var mainPlayerId, out var otherPlayerIds, out var allPlayerIds);
            var cardModelsProvider = NewCardModels();
            var matchInfo = new MatchInfo(new PlayersInfo(mainPlayerId, otherPlayerIds), cardModelsProvider.CardModels);

            return new QosGame(
                () => new LocalHostGameWorld(new GameWorld(allPlayerIds, cardModelsProvider), mainPlayerId, NewAiBots(otherPlayerIds)), 
                () => new GraphicSceneInteractor(_CreateGraphicSceneFactory(), matchInfo));
        }


        private void NewPlayerIds(out PlayerId mainPlayerId, out List<PlayerId> otherPlayerIds, out List<PlayerId> allPlayerIds)
        {
            mainPlayerId = PlayerId.GenerateNew();
            const int PLAYERS_COUNT = 5;
            otherPlayerIds = Enumerable.Range(1, PLAYERS_COUNT).Select(_ => PlayerId.GenerateNew()).ToList();

            allPlayerIds = otherPlayerIds.Select(id => id).ToList();
            allPlayerIds.Add(mainPlayerId);
        }


        private Dictionary<PlayerId, AbstractGameBot> NewAiBots(IEnumerable<PlayerId> otherPlayerIds)
        {
            return otherPlayerIds.ToDictionary(
                id => id,
                id => new StandartGameBot(id) as AbstractGameBot);
        }


        private ICardModelsProvider NewCardModels()
        {
            return new StandartPlayingCardModelsProvider();
            //return new FewStandartPlayingCardModelsProvider();
        }


    }
}
