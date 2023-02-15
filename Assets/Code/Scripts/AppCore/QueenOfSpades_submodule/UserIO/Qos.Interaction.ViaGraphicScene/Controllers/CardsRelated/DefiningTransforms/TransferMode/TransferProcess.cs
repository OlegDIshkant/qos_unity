using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    ///  онтроллер, вычисл€ющий расположени€ карт игрока, когда другой игрок выбирает, а затем забирает себе какие-то из них.
    /// </summary>
    /// <remarks> 
    /// ќдин экземпл€р класса должен заниматьс€ не более, чем одним таким процессом передачи между двум€ игроками (а не всеми сразу).
    /// »гроки, учавствующие в этом процессе, могут быть любыми.
    /// </remarks>
    public class TransferProcess : ControllersController, IDictionaryDataProvider<CardId, Transform>, IHashSetDataProvider<CardId>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTFormsProvider;
        private readonly IHashSetDataProvider<CardId> _selectedCardsProvider;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTFormsProvider.DictionaryOutput;
        public ListData<CardId> HashSetOutput => _selectedCardsProvider.HashSetOutput;


        public TransferProcess(
            Contexts contexts,
            int processIndex,
            IDictionaryDataProvider<int, TransferProcessInfo> transferPrcossesInfoProvider,
            ICursorController cursor,
            ICameraController cam,
            IPlayersTFormsDefiner playersTFormsDefiner,
            HighlightSettings highlightSettings) : 
            base(contexts)
        {
            var queue = new ControllersQueue();

            var processInfo = queue.AddWithoutTag(new TransferParamsProvider(contexts, processIndex, transferPrcossesInfoProvider));

            var p_to_mp_tForms = queue.AddWithoutTag(new P_to_MP_TransferModeTForms(contexts, processInfo, cursor, cam, playersTFormsDefiner, highlightSettings));
            var mp_to_p_tForms = queue.AddWithoutTag(new MP_to_P_TransferModeTForms(contexts, processInfo, cam, playersTFormsDefiner));
            var p_to_p_tForms = queue.AddWithoutTag(new P_to_P_TransferModeTForms(contexts, processInfo, playersTFormsDefiner));

            var selectedTForms = queue.AddWithoutTag(new TransferTFormsSelector(contexts, processInfo, p_to_p_tForms, p_to_mp_tForms, mp_to_p_tForms));

            SetControllersQueue(queue);

            _finalTFormsProvider = selectedTForms;
            _selectedCardsProvider = p_to_mp_tForms;
        }
    }


}