using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated
{
    /// <summary>
    /// Контроллер, определяющий, какие карты выбраны (для этого проверяется, есть ли у них выделение курсором и какого типа). 
    /// </summary>
    public class SelectedCardsExtractor : AbstractController, IHashSetDataProvider<CardId>
    {

        private readonly IDictionaryDataProvider<CardId, HighlightsData> _cardHighlightsProvider;
        private readonly HighlightSettings _highlightSettings;

        
        private ListData<CardId>.Editable _selectedCardsOutput;
        public ListData<CardId> HashSetOutput { get; private set; }

        

        public SelectedCardsExtractor(
            Contexts contexts, 
            IDictionaryDataProvider<CardId, HighlightsData> cardHighlightsProvider, 
            HighlightSettings highlightSettings) : 
            base(contexts)
        {
            _cardHighlightsProvider = cardHighlightsProvider;
            _highlightSettings = highlightSettings;

            HashSetOutput = new ListData<CardId>(out _selectedCardsOutput);
        }




        public override void Update()
        {
            RemoveDeSelectedCards();
            AddRecentlySelectedCards();
        }


        private void AddRecentlySelectedCards()
        {
            foreach (var (card, highlight) in NewCardHighlights())
            {
                if (IsHighlightedAsSelectedCard(highlight))
                {
                    RememberCard(card);
                }
            }
        }


        private IEnumerable<(CardId, HighlightsData)> NewCardHighlights()
        {
            if (_cardHighlightsProvider.DictionaryOutput?.HasChanged ?? false)
            {
                foreach (var item in _cardHighlightsProvider.DictionaryOutput.AddedOrChanged)
                {
                    yield return (item.Key, item.Value);
                }
            }
        }


        private bool IsHighlightedAsSelectedCard(HighlightsData highlight) => highlight.Highlight.Equals(_highlightSettings.SelectHighlight);



        private void RememberCard(CardId card)
        {
            Logger.Verbose($"Карта '{card}' теперь считается выбранной пользователем для сброса.");
            _selectedCardsOutput.AddItem(card);
        }


        private void RemoveDeSelectedCards()
        {
            foreach (var card in NoMoreHighlightedCards())
            {
                if (CardWasSelected(card))
                {
                    ForgetInfoForCard(card);
                }
            }
        }


        private IEnumerable<CardId> NoMoreHighlightedCards()
        {
            if (_cardHighlightsProvider.DictionaryOutput?.HasChanged ?? false)
            {
                foreach (var card in _cardHighlightsProvider.DictionaryOutput.Removed)
                {
                    yield return card;
                }
            }
        }

        private bool CardWasSelected(CardId card) => HashSetOutput.Items.Contains(card);


        private void ForgetInfoForCard(CardId card)
        {
            Logger.Verbose($"Забываем информацию о карте '{card}'.");
            _selectedCardsOutput.RemoveItem(card);
        }


    }




}