using Qos.Domain;
using Qos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Suits = Qos.Domain.StandartPlayingCardModel.Suits;
using Values = Qos.Domain.StandartPlayingCardModel.Values;


namespace Qos.GameLogic.GameWorld.Stages
{
    /// <summary>
    /// Генерирует по одной из всех возможных <see cref="StandartPlayingCardModel"/> (кроме королевы червей).
    /// </summary>
    public class StandartPlayingCardModelsProvider : ICardModelsProvider
    {
        public IReadOnlyDictionary<CardId, CardModel> CardModels { get; private set; }

        public StandartPlayingCardModelsProvider()
        {
            var result = new Dictionary<CardId, CardModel>();

            foreach (Suits suit in Enum.GetValues(typeof(Suits)))
            {
                foreach (Values value in Enum.GetValues(typeof(Values)))
                {
                    if (suit == Suits.HEARTS && value == Values.QUEEN)
                    {
                        continue;
                    }

                    var id = CardId.GenerateNew();
                    var model = new StandartPlayingCardModel(suit, value);
                    result.Add(id, model);
                }
            }

            CardModels = new ReadOnlyDictionary<CardId, CardModel>(result);
        }
    }



    /// <summary>
    /// Генерирует урзанный набор возможных <see cref="StandartPlayingCardModel"/> (кроме королевы червей).
    /// </summary>
    public class FewStandartPlayingCardModelsProvider : ICardModelsProvider
    {
        public IReadOnlyDictionary<CardId, CardModel> CardModels { get; private set; }

        public FewStandartPlayingCardModelsProvider()
        {
            var result = new Dictionary<CardId, CardModel>();

            foreach (Suits suit in Enum.GetValues(typeof(Suits)))
            {
                result.Add(CardId.GenerateNew(), new StandartPlayingCardModel(suit, Values.ACE));
                result.Add(CardId.GenerateNew(), new StandartPlayingCardModel(suit, Values.KING));
                if (suit != Suits.HEARTS)
                {
                    result.Add(CardId.GenerateNew(), new StandartPlayingCardModel(suit, Values.QUEEN));
                }
                result.Add(CardId.GenerateNew(), new StandartPlayingCardModel(suit, Values.JACK));
            }

            CardModels = new ReadOnlyDictionary<CardId, CardModel>(result);
        }
    }
}
