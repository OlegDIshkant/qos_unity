using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated
{
    public interface ICardsController : ITFormsProvider<CardId>
    {
        ImmutableDictionary<PlayerId, ICard_ReadOnly> CardsInfo { get; }
    }



    public class CardsController : AbstractController, ICardsController
    {
        public event Action<CardId, Transform> OnTransformChanged;

        private readonly Func<CardModel, ICard> _CardFactoryMethod;
        private readonly ICardTformsDefiner _cardTFormsProvider;
        private readonly IObjectsSpawnDirector<CardId> _cardsSpawnDirector;
        private readonly IModelsProvider<CardId, CardModel> _cardsModelsProvider;

        private Dictionary<CardId, ICard> _cards = new Dictionary<CardId, ICard>();


        public ImmutableDictionary<PlayerId, ICard_ReadOnly> CardsInfo => throw new NotImplementedException();

        public ImmutableDictionary<CardId, ITransformReadOnly> TransformInfo => throw new NotImplementedException();


        public CardsController(
            Contexts context,
            Func<CardModel, ICard> CardFactoryMethod,
            ICardTformsDefiner cardTFormsProvider,
            IObjectsSpawnDirector<CardId> cardsSpawnDirector,
            IModelsProvider<CardId, CardModel> cardsModelsProvider) :
            base(context)
        {
            _CardFactoryMethod = CardFactoryMethod;
            _cardTFormsProvider = cardTFormsProvider;
            _cardsSpawnDirector = cardsSpawnDirector;
            _cardsModelsProvider = cardsModelsProvider;
        }


        public override void Update()
        {
            foreach (var cardId in CardsToSpawn())
            {
                SpawnNewCards(cardId);
            }
            UpdateCards(); 
        }


        private IEnumerable<CardId> CardsToSpawn()
        {
            if (_cardsSpawnDirector.ShouldBeSpawnedNow?.HasChanged ?? false)
            {
                return _cardsSpawnDirector.ShouldBeSpawnedNow.Added;
            }
            return Enumerable.Empty<CardId>();
        }


        private void SpawnNewCards(CardId cardId)
        {
            var cardModel = _cardsModelsProvider.Models[cardId];
            Logger.Verbose($"Создаем новую карту с id '{cardId}' и представлением '{cardModel}'.");
            _cards.Add(cardId, _CardFactoryMethod(cardModel));
        }


        private void UpdateCards()
        {
            foreach (var item in _cards)
            {
                ChangeTransform(item.Key, item.Value, _cardTFormsProvider.TFormsToApply.Items[item.Key]);
            }
        }


        private void ChangeTransform(CardId cardId, ICard card, Transform target)
        {
            card.SetTransform(target);
            OnTransformChanged?.Invoke(cardId, target);
        }



    }


}
