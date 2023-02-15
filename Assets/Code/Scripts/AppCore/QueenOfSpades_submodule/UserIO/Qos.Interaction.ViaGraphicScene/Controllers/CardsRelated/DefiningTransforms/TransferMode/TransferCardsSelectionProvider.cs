using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Linq;

namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{

    /// <summary>
    /// ќпредел€ет, карту дл€ передачи главному игроку.
    /// </summary>
    public class TransferCardsSelectionProvider : AbstractController , IHashSetDataProvider<CardId>
    {
        private readonly IDictionaryDataProvider<int, TransferProcessInfo> _transferProcessesScheduler;

        private Dictionary<int, IHashSetDataProvider<CardId>> _sources;
        private IHashSetDataProvider<CardId> _currentSource;

        private ListData<CardId>.Editable _output;
        public ListData<CardId> HashSetOutput { get; private set; }

        private ListData<CardId> CurrentSource => _currentSource?.HashSetOutput;


        public TransferCardsSelectionProvider(
            Contexts contexts,
            IDictionaryDataProvider<int, TransferProcessInfo> transferProcessesScheduler,
            Dictionary<int, IHashSetDataProvider<CardId>> sources) :
            base(contexts)
        {
            _transferProcessesScheduler = transferProcessesScheduler;
            _sources = sources;
            HashSetOutput = new ListData<CardId>(out _output);
        }


        public override void Update()
        {
            if (ShouldChangeSource(out var index))
            {
                ChangeSource(index);
            }

            UpdateCardsFromSource();
        }


        private bool ShouldChangeSource(out int indexForNewSource)
        {
            if (_transferProcessesScheduler.DictionaryOutput.HasChanged)
            {
                foreach (var newActiveProcessIndex in _transferProcessesScheduler.DictionaryOutput.AddedOrChanged.Keys)
                {
                    indexForNewSource = newActiveProcessIndex;
                    return true;
                }
            }
            indexForNewSource = default;
            return false;
        }


        private void ChangeSource(int index)
        {
            if (HasSameIndexAlready(index))
            {
                return;
            }
            _currentSource = _sources[index];
            ForgetPrevioslySelectedCards();
        }


        private void ForgetPrevioslySelectedCards()
        {
            var toDelete = HashSetOutput.Items.ToList();

            foreach (var card in toDelete)
            {
                _output.RemoveItem(card);
            }
        }


        private bool HasSameIndexAlready(int index)
        {
            return _currentSource != null ?
                _currentSource == _sources[index] :
                false;
        }


        private void UpdateCardsFromSource()
        {
            if (CurrentSource?.HasChanged ?? false)
            {
                foreach (var added in CurrentSource.Added)
                {
                    RememberCard(added);
                }

                foreach (var removed in CurrentSource.Removed)
                {
                    ForgetCard(removed);
                }
            }
        }


        private void RememberCard(CardId card)
        {
            _output.AddItem(card);
        }


        private void ForgetCard(CardId card)
        {
            _output.RemoveItem(card);
        }
    }
}