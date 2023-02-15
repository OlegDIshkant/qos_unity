using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий расположения карт главного игрока (пользователя), когда другой игрок выбирает, а затем забирает себе какие-то из них.
    /// </summary>
    public class MP_to_P_TransferModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTFormsProvider;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTFormsProvider.DictionaryOutput;


        public MP_to_P_TransferModeTForms(
            Contexts contexts,
            ISingleDataProvider<TransferProcessInfo> processInfoProvider,
            ICameraController cam,
            IPlayersTFormsDefiner playersTFormsDefiner) :
            base(contexts)
        {
            var queue = new ControllersQueue();
            var canonTForms = queue.AddWithoutTag(new MainPlayerToPlayer_TransferMode_CanonCardsTFormsController(contexts, processInfoProvider, playersTFormsDefiner, cam));
            var highlightedTForms = queue.AddWithoutTag(new MainPlayerToPlayer_CardTForms_TransferMode_Modifier(contexts, canonTForms));
            var smoothedTForms = queue.AddWithoutTag(new SmoothTFormTransitModifier<CardId>(contexts, 0.003f, highlightedTForms));
            var floatingTForms = queue.AddWithoutTag(new MainPlayer_CardTForm_FloatAnimModifier(contexts, smoothedTForms));
            SetControllersQueue(queue);

            _finalTFormsProvider = floatingTForms;
        }
    }
}