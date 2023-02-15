using Qos.Domain.Entities;
using System.Collections.Generic;


namespace Qos.GameLogic.GameWorld.Stages
{
    public interface ICardModelsProvider
    {
        IReadOnlyDictionary<CardId, CardModel> CardModels { get; }
    }
}