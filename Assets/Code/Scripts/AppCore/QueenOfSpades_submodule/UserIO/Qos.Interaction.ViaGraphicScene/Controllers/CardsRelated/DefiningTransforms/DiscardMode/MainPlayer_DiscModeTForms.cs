using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должны иметь карты главного игрока (пользователя), когда тот выбирает среди них те, что собирается сбросить.
    /// </summary>
    public class MainPlayer_DiscModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>, IHashSetDataProvider<CardId>
    {

        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;
        private readonly IHashSetDataProvider<CardId> _cardsToDiscardProvider;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;
        public ListData<CardId> HashSetOutput => _cardsToDiscardProvider.HashSetOutput;


        public MainPlayer_DiscModeTForms(
            Contexts contexts,
            PlayerId mainPlayerId,
            ICursorController cursor,
            ICameraController cam,
            ICardsActionsProvider cardsActionsProvider,
            HighlightSettings highlightSettings) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var canonTForms = queue.AddWithoutTag(new MainPlayerDiscModeCanonCardsTFormsController(contexts, mainPlayerId, cam, cardsActionsProvider));
            var modByCursorTForms = queue.AddWithoutTag(new MainPlayer_CardTForms_DiscMode_ByCursor_Modifier(contexts, cam, cursor, canonTForms, mainPlayerId, cardsActionsProvider, highlightSettings));
            var smoothedTForms = queue.AddWithoutTag(new SmoothTFormTransitModifier<CardId>(contexts, 0.003f, modByCursorTForms));
            var floatingTForms = queue.AddWithoutTag(new MainPlayer_CardTForm_FloatAnimModifier(contexts, smoothedTForms));

            var selectedCardsExtractor = queue.AddWithoutTag(new SelectedCardsExtractor(contexts, modByCursorTForms, highlightSettings));

            SetControllersQueue(queue);

            _finalTForms = floatingTForms;
            _cardsToDiscardProvider = selectedCardsExtractor;
        }
    }
}