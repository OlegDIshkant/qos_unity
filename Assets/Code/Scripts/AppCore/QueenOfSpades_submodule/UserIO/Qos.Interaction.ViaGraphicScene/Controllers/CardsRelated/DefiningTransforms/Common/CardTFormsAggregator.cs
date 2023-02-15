using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    ///  онтроллер, агрегирующий результаты работы нескольких определителей положений карт.
    /// </summary>
    public class CardTFormsAgregator<Key> : AbstractController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly Dictionary<Key, IDictionaryDataProvider<CardId, Transform>> _tFormProviders;

        private DictionaryData<CardId, Transform>.Editable _dataEdit;
        public DictionaryData<CardId, Transform> DictionaryOutput { get; private set; }


        public CardTFormsAgregator(
            Contexts contexts,
            Dictionary<Key, IDictionaryDataProvider<CardId, Transform>> tFormProviders) :
            base(contexts)
        {
            _tFormProviders = tFormProviders;

            //SetUpExistingTForms();

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