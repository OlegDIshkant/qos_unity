using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должны иметь карты игрока, когда другой игрок выбрает среди них те, что заберет себе.
    /// </summary>
    public class DefiningCardsTransferModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>, IHashSetDataProvider<CardId>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;
        private readonly IHashSetDataProvider<CardId> _selectedCardsProvider;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;
        public ListData<CardId> HashSetOutput => _selectedCardsProvider.HashSetOutput;


        public DefiningCardsTransferModeTForms(
            Contexts contexts,
            ICardsActionsProvider cardsActions,
            ICursorController cursor,
            ICameraController cam,
            IPlayersTFormsDefiner playerTFormsDefiner,
            HighlightSettings highlightSettings) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var scheduler = queue.AddWithoutTag(new TransferProcessesScheduler(contexts, cardsActions));

            var processes = new Dictionary<int, IDictionaryDataProvider<CardId, Transform>>();
            for (int i = 0; i < contexts.PlayersInfo.AllPlayersAmount; i++)
            {
                processes.Add(i, queue.AddWithoutTag(new TransferProcess(contexts, i, scheduler, cursor, cam, playerTFormsDefiner, highlightSettings)));
            }

            var aggregator = queue.AddWithoutTag(new CardTFormsAgregator<int>(contexts, processes));

            var selectedCardsAggregator = queue.AddWithoutTag(new TransferCardsSelectionProvider(contexts, scheduler, 
                processes.ToDictionary(i => i.Key, i => (IHashSetDataProvider<CardId>)(object)i.Value)));

            SetControllersQueue(queue);

            _finalTForms = aggregator;
            _selectedCardsProvider = selectedCardsAggregator;
        }


    }



}