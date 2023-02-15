using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, агрегирующий результаты работы нескольких контроллеров, определяющих положения карт, когда игроки выбирают среди них те, что будут сбрасывать.
    /// </summary>
    public class CardDiscModeTFormsAgregator : AbstractController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly ICardsActionsProvider _cardsActionsProvider;
        private readonly Dictionary<PlayerId, IDictionaryDataProvider<CardId, Transform>> _tFormProviders;

        private DictionaryData<CardId, Transform>.Editable _dataEdit;
        public DictionaryData<CardId, Transform> DictionaryOutput { get; private set; }


        public CardDiscModeTFormsAgregator(
            Contexts contexts,
            ICardsActionsProvider cardsActionsProvider,
            Dictionary<PlayerId, IDictionaryDataProvider<CardId, Transform>> idleTFormProviders) :
            base(contexts)
        {
            _cardsActionsProvider = cardsActionsProvider;
            _tFormProviders = idleTFormProviders;

            DictionaryOutput = new DictionaryData<CardId, Transform>(out _dataEdit);
        }


        public override void Update()
        {
            foreach (var item in FindCardsToUpdate())
            {
                UpdateCardTForm(item.Key, item.Value);
            }
        }


        private void UpdateCardTForm(CardId cardId, Transform newTransform) => _dataEdit.SetItem(cardId, newTransform);


        private IEnumerable<KeyValuePair<CardId, Transform>> FindCardsToUpdate()
        {
            foreach (var provider in _tFormProviders)
            {
                var tForms = provider.Value.DictionaryOutput;
                if (tForms.HasChanged)
                {
                    foreach (var changed in tForms.AddedOrChanged)
                    {
                        yield return changed;
                    }
                }
            }
        }


    }
}