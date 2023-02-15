using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий расположения карт игрока, когда главный игрок (пользователь) выбирает, а затем забирает себе какие-то из них.
    /// </summary>
    public class P_to_MP_TransferModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>, IHashSetDataProvider<CardId>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTFormsProvider;
        private readonly IHashSetDataProvider<CardId> _selectedCardsProvider;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTFormsProvider.DictionaryOutput;
        public ListData<CardId> HashSetOutput => _selectedCardsProvider.HashSetOutput;


        public P_to_MP_TransferModeTForms(
            Contexts contexts,
            ISingleDataProvider<TransferProcessInfo> processInfoProvider,
            ICursorController cursor,
            ICameraController cam,
            IPlayersTFormsDefiner playersTFormsDefiner,
            HighlightSettings highlightSettings) : 
            base(contexts)
        {
            var queue = new ControllersQueue();
            var canonTForms = queue.AddWithoutTag(new PlayerToMainPlayer_TransferMode_CanonCardsTFormsController(contexts, processInfoProvider, playersTFormsDefiner, cam));
            var highlightedTForms = queue.AddWithoutTag(new PlayerToMainPlayer_CardTForms_TransferMode_Modifier(contexts, cam, cursor, highlightSettings, canonTForms, processInfoProvider));
            var smoothedTForms = queue.AddWithoutTag(new SmoothTFormTransitModifier<CardId>(contexts, 0.003f, highlightedTForms));
            var floatingTForms = queue.AddWithoutTag(new MainPlayer_CardTForm_FloatAnimModifier(contexts, smoothedTForms));
            SetControllersQueue(queue);

            var selectedCardsExtractor = queue.AddWithoutTag(new SelectedCardsExtractor(contexts, highlightedTForms, highlightSettings));

            _finalTFormsProvider = floatingTForms;
            _selectedCardsProvider = selectedCardsExtractor;
        }
    }
}