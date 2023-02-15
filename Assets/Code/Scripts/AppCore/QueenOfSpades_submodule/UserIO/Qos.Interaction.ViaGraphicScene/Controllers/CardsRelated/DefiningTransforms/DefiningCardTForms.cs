using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    public interface ICardTformsDefiner
    {
        /// <summary>
        /// ѕоложени€ в пространстве, которые должны быть в данный момент у карт.
        /// </summary>
        DictionaryData<CardId, Transform> TFormsToApply { get; }
        /// <summary>
        ///  арты, которые главный игрок (пользователь) выделил дл€ сброса.
        /// </summary>
        ListData<CardId> CardsToDiscardByMainPlayer { get; }
        /// <summary>
        ///  арты, которые главный игрок (пользователь) выделил дл€ передачи себе.
        /// </summary>
        ListData<CardId> CardsToTransferToMainPlayer { get; }
    }


    /// <summary>
    ///  онтроллер, вычисл€ющий, какое расположение в пространстве должна в данный момент времени иметь та или ина€ карта.
    /// </summary>
    public class DefiningCardsTForms : ControllersController, ICardTformsDefiner
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTFormsProvider;
        private readonly IHashSetDataProvider<CardId> _cardsToDiscardProvider;
        private readonly IHashSetDataProvider<CardId> _cardsToTransferToMainPlayer;

        public DictionaryData<CardId, Transform> TFormsToApply => _finalTFormsProvider.DictionaryOutput;
        public ListData<CardId> CardsToDiscardByMainPlayer => _cardsToDiscardProvider.HashSetOutput;
        public ListData<CardId> CardsToTransferToMainPlayer => _cardsToTransferToMainPlayer.HashSetOutput;


        public DefiningCardsTForms(
            Contexts contexts,
            ICardsActionsProvider cardsActions,
            IDeckController deck,
            ICardHeapController cardHeap,
            ICursorController cursor,
            ICameraController cam,
            IPlayersTFormsDefiner playerTFormsDefiner,
            IDictionaryDataProvider<CardId, Transform> prevCardsTFormsProvider,
            HighlightSettings highlightSettings) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var inDeckTforms = queue.AddWithoutTag(new InDeck_CardTForms_Controller(contexts, deck, cardsActions));
            var inHeapTforms = queue.AddWithoutTag(new InHeap_CardTForms_Controller(contexts, new InHeap_TFormCalcer(), cardHeap, cardsActions));
            var idleTforms = queue.AddWithoutTag(new DefiningCardsIdleTForms(contexts, cardsActions, cursor, cam, playerTFormsDefiner));
            var discModeTforms = queue.AddWithoutTag(new DefiningCardsDiscModeTForms(contexts, cardsActions, cursor, cam, playerTFormsDefiner));
            var transferModeTforms = queue.AddWithoutTag(new DefiningCardsTransferModeTForms(contexts, cardsActions, cursor, cam, playerTFormsDefiner, highlightSettings));
            var hideModeTforms = queue.AddWithoutTag(new DefiningCardsHideModeTForms(contexts, cardsActions, cursor, cam, playerTFormsDefiner));

            var transitTforms = queue.AddWithoutTag(new CardsTransitTFormsDefiner(contexts, cardsActions, prevCardsTFormsProvider, inDeckTforms, inHeapTforms, idleTforms, discModeTforms, transferModeTforms, hideModeTforms));
            var tFormsSelector = queue.AddWithoutTag(new CardTFormsSelector(contexts, cardsActions, inDeckTforms, inHeapTforms, idleTforms, discModeTforms, transferModeTforms, hideModeTforms, transitTforms));

            SetControllersQueue(queue);

            _finalTFormsProvider = tFormsSelector;
            _cardsToDiscardProvider = discModeTforms;
            _cardsToTransferToMainPlayer = transferModeTforms;
        }
    }

}