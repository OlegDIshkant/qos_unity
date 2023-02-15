using Qos.Domain.Entities;
using System.Collections.Generic;


namespace Qos.GameLogic.GameWorld.Stages
{
    internal interface ICardsShaffler
    {
        IEnumerable<CardId> Shaffle(IEnumerable<CardId> cards);
    }
}
