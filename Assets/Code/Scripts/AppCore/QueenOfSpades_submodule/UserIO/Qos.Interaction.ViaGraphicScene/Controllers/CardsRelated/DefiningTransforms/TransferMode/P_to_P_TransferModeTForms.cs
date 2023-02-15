using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий расположения карт игрока, когда другой игрок выбирает, а затем забирает себе какие-то из них.
    /// </summary>
    public class P_to_P_TransferModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTFormsProvider;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTFormsProvider.DictionaryOutput;


        public P_to_P_TransferModeTForms(
            Contexts contexts,
            ISingleDataProvider<TransferProcessInfo> processInfoProvider,
            IPlayersTFormsDefiner playersTFormsDefiner) :
            base(contexts)
        {
            var queue = new ControllersQueue();
            var canonTForms = queue.AddWithoutTag(new PlayerToPlayer_TransferMode_CanonCardsTFormsController(contexts, processInfoProvider, playersTFormsDefiner));
            var highlightedTForms = queue.AddWithoutTag(new PlayerToPlayer_CardTForms_TransferMode_Modifier(contexts, canonTForms));
            var smoothedTForms = queue.AddWithoutTag(new SmoothTFormTransitModifier<CardId>(contexts, 0.003f, highlightedTForms));
            var floatingTForms = queue.AddWithoutTag(new MainPlayer_CardTForm_FloatAnimModifier(contexts, smoothedTForms));
            SetControllersQueue(queue);

            _finalTFormsProvider = floatingTForms;
        }
    }
}