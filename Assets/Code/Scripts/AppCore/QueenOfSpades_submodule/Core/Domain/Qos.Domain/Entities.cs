using System;
using System.Collections.Generic;

namespace Qos.Domain.Entities
{
    public interface IMatchId : IEquatable<IMatchId> { }

    public struct MatchId : IMatchId
    {
        public bool Equals(IMatchId other)
        {
            throw new NotImplementedException();
        }
    }


    public struct PlayerId
    {
        private static int counter = 0;
        private int _id;

        public static PlayerId GenerateNew() => new PlayerId() { _id = counter++ };


        public override bool Equals(object obj)
        {
            if (obj is PlayerId other)
            {
                return this._id == other._id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Игрок #" + _id.ToString();
        }
    }


    /// <summary>
    /// Модель любого участника игры.
    /// </summary>
    public struct PlayerModel
    {
        public string Name { get; private set; }

        public static PlayerModel GenerateNew(string name)
        {
            return new PlayerModel()
            {
                Name = name
            };
        }
    }



    public class CardModel
    {
        public string DeckName { get; private set; }
        public string CardName { get; private set; }

        public CardModel(string deckName, string cardName)
        {
            DeckName = deckName;
            CardName = cardName;
        }   
    }


    public struct CardId
    {
        private static int counter = 0;

        private int _id;

        public static CardId GenerateNew() => new CardId() { _id = counter++ };


        public override bool Equals(object obj)
        {
            if (obj is CardId other)
            {
                return this._id == other._id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public override string ToString() => _id.ToString();
    }


    public class CardGroup : HashSet<CardId> 
    {
        public CardGroup() : base() { }

        public CardGroup(IEnumerable<CardId> cardIds) : base(cardIds) { }
    }

    public class CardGroups : List<CardGroup>
    {
        public CardGroups() : base() { }

        public CardGroups(IEnumerable<CardGroup> groups) : base(groups) { }
    }
}
