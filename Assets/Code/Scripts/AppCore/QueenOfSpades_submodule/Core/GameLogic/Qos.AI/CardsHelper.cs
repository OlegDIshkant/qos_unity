using Qos.Domain;
using Qos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Qos.AI
{
    /// <summary>
    /// Помогает следить за картами игрока.
    /// </summary>
    internal class CardsHelper
    {
        private Dictionary<CardId, CardModel> _cardModels = new Dictionary<CardId, CardModel>();
        private HashSet<CardId> _playerCards = new HashSet<CardId>();
        private Random _rnd = new Random();

        public void SetPlayerCards(IEnumerable<CardId> cards)
        {
            foreach (var cardId in cards)
            {
                _playerCards.Add(cardId);
            }
        }


        public void RemoveSomePlayerCards(IEnumerable<CardId> cardsToRemove)
        {
            foreach (var cardId in cardsToRemove)
            {
                _playerCards.Remove(cardId);
            }
        }


        public void RememberModel(CardId cardId, CardModel model)
        {
            _cardModels[cardId] = model;
        }


        public CardGroups FindCardsToDiscard()
        {
            return new CardGroups(
                _playerCards
                .ToDictionary(cardId => cardId, cardId => _cardModels[cardId])
                .Where(item => item.Value is StandartPlayingCardModel)
                .Where(item => !IsUnPairable((StandartPlayingCardModel)item.Value))
                .GroupBy(item => ((StandartPlayingCardModel)item.Value).Value)
                .Where(group => group.Count() == 2)
                .Select(group => new CardGroup(group.Select(item => item.Key))));
        }


        private bool IsUnPairable(StandartPlayingCardModel card) =>
            card.Suit == StandartPlayingCardModel.Suits.SPADES &&
            card.Value == StandartPlayingCardModel.Values.QUEEN;


        public bool HasCardsToDiscard()
        {
            return FindCardsToDiscard().Count > 0;
        }


        public bool HasCardsToDiscard(out CardGroups cardsToDiscard)
        {
            cardsToDiscard = FindCardsToDiscard();
            return cardsToDiscard.Count > 0;
        }


        public CardId FindCardToTransfer(IEnumerable<CardId> possibleCards)
        {
            var rndIndex = _rnd.Next(0, possibleCards.Count());
            return possibleCards.ElementAt(rndIndex); // пока алгоритм выбора оставим примитивным
        }
    }
}
