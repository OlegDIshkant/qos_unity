using Qos.Domain.Entities;
using System.Collections.Generic;


namespace Qos.GameLogic.GameWorld.Activities
{
    public interface ICardMatchingStrategy
    {
        bool CanBeMatched(IEnumerable<CardModel> cards);
    }
}