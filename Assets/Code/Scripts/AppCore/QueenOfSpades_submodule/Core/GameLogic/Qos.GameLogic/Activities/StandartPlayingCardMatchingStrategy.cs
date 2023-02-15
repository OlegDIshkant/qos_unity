using Qos.Domain;
using Qos.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using static Qos.Domain.StandartPlayingCardModel;


namespace Qos.GameLogic.GameWorld.Activities
{
    internal class StandartPlayingCardMatchingStrategy : ICardMatchingStrategy
    {
        public bool CanBeMatched(IEnumerable<CardModel> cards)
        {
            if (AreTwoStandartCards(cards, out var first, out var second))
            {
                return CanMakeCouple(first, second);
            }
            return false;
        }


        private bool AreTwoStandartCards(IEnumerable<CardModel> cards, out StandartPlayingCardModel firstCard, out StandartPlayingCardModel secondCard)
        {
            if (cards.Count() == 2)
            {
                var first = cards.First();
                var second = cards.Skip(1).First();
                if (first is StandartPlayingCardModel fC && second is StandartPlayingCardModel sC)
                {
                    firstCard = fC;
                    secondCard = sC;
                    return true;
                }
            }
            firstCard = secondCard = null;
            return false;
        }


        public bool CanMakeCouple(StandartPlayingCardModel cardA, StandartPlayingCardModel cardB)
        {
            if (IsUnMatchable(cardA) || IsUnMatchable(cardB))
            {
                return false;
            }
            return HaveSameValue(cardA, cardB);
        }


        public bool IsUnMatchable(StandartPlayingCardModel card) =>
            card.Suit == Suits.SPADES &&
            card.Value == Values.QUEEN;

        private bool HaveSameValue(StandartPlayingCardModel cardA, StandartPlayingCardModel cardB) =>
            cardA.Value == cardB.Value;



    }
}