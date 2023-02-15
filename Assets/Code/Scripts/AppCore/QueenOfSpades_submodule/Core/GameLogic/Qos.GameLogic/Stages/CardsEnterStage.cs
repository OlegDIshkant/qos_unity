using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.GameLogic.GameWorld.Activities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Qos.GameLogic.GameWorld.Helper;


namespace Qos.GameLogic.GameWorld.Stages
{
    /// <summary>
    /// Этап, когда в игре появляются карты и распределяются между игроками.
    /// </summary>
    internal class CardsEnterStage : AbstractStage
    {
        private ICardModelsProvider _cardModelsProvider;
        private TimeContext _timeContext;
        private IEnumerable<PlayerId> _playerIds;
        private PlayersCards _playerCards;
        private ICardsShaffler _cardsShaffler = new TestCardsShaffler();
        private List<CardId> _cardsDeckOrder;


        public CardsEnterStage(
            ICardModelsProvider cardModelsProvider, 
            IEnumerable<PlayerId> playerIds, 
            TimeContext timeContext, 
            PlayersCards playerCards)
        {
            _cardModelsProvider = cardModelsProvider;
            _playerIds = playerIds;
            _timeContext = timeContext;
            _playerCards = playerCards;
        }


        public override IEnumerator Complete()
        {
            yield return CardsCreating();
            yield return CardsDistribution();
        }


        private IEnumerator CardsCreating()
        {
            _cardsDeckOrder = new List<CardId>(_cardsShaffler.Shaffle(_cardModelsProvider.CardModels.Keys));

            foreach (var cardId in _cardsDeckOrder )
            {
                var cardModel = _cardModelsProvider.CardModels[cardId];
                yield return RaiseContiniousEvent(new CardCreatedEvent(cardId, cardModel), 10, _timeContext);
            }

        }


        private IEnumerator CardsDistribution()
        {
            var noMoreCardsToEnqueue = false;
            var cardsToDistribute = new CardsToDistributeQueue();

            yield return IteratateSimultaneously(
                EnqueueingCardsToDistribute(cardsToDistribute, OnAllCardsEnqueued: () => noMoreCardsToEnqueue = true),
                DequeueingAndDistributingCards(cardsToDistribute, KeepWaitingForNewCards: () => !noMoreCardsToEnqueue));
        }


        private IEnumerator EnqueueingCardsToDistribute(CardsToDistributeQueue cardsToDistribute, Action OnAllCardsEnqueued)
        {
            var playersIterator = _playerIds.IterateInCycle();

            foreach (var cardId in Enumerable.Reverse(_cardsDeckOrder))
            {
                var item = (
                    player: playersIterator.GetNext(),
                    cards: cardId.WrapInNewList()
                    );
                cardsToDistribute.Enqueue(item);
                yield return null;
            }

            OnAllCardsEnqueued?.Invoke();
        }


        private IEnumerator DequeueingAndDistributingCards(CardsToDistributeQueue queue, Func<bool> KeepWaitingForNewCards)
        {
            var distributingRoutines = new RoutinePool();

            while (KeepWaitingForNewCards() || distributingRoutines.HasWorkToDo || queue.Count > 0)
            {
                DequeueToStartDistributing(queue, distributingRoutines);
                yield return distributingRoutines.Iterate();

            }
        }


        private void DequeueToStartDistributing(CardsToDistributeQueue cardsToDistribute, RoutinePool routinePool)
        {
            while (cardsToDistribute.Count > 0)
            {
                var item = cardsToDistribute.Dequeue();
                routinePool.AddRoutine(DistributingCardsToPlayer(item.player, item.cards));
            }
        }


        private IEnumerator DistributingCardsToPlayer(PlayerId playerId, IEnumerable<CardId> cardIds)
        {
            yield return RaiseContiniousEvent(new PlayerGettingCardsEvent(playerId, cardIds), 100, _timeContext);
            AssignNewCardsToPlayer(playerId, cardIds);
            yield return new PlayerHasCardsEvent(playerId, GetAllAssignedCardsFor(playerId));
        }


        private void AssignNewCardsToPlayer(PlayerId playerId, IEnumerable<CardId> cardIds)
        {
            if (!_playerCards.ContainsKey(playerId))
            {
                _playerCards[playerId] = new HashSet<CardId>();
            }

            foreach (var cardId in cardIds)
            {
                _playerCards[playerId].Add(cardId);
            }
        }


        private IEnumerable<CardId> GetAllAssignedCardsFor(PlayerId playerId)
        {
            if (!_playerCards.ContainsKey(playerId))
            {
                _playerCards[playerId] = new HashSet<CardId>();
            }

            return _playerCards[playerId];
        }

    }



    internal class CardsToDistributeQueue : Queue<(PlayerId player, IEnumerable<CardId> cards)> { }


}
