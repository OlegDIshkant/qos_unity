using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контролер, определяющий канонические положения карт главного игрока (пользователя), когда другой игрок выбирает среди них карты для передачи себе.
    /// </summary>
    public class MainPlayerToPlayer_TransferMode_CanonCardsTFormsController : TransferMode_CanonTForms_Controller
    {
        private readonly IPlayersTFormsDefiner _playersTFormsDefiner;
        private readonly ICameraController _cam;

        private PlayerId _taker;

        private Transform TakerTForm => _playersTFormsDefiner.TFormsToApply.Items[_taker];

        protected override TransferType TransferType => TransferType.FROM_MAIN_PLAYER_TO_PLAYER;


        public MainPlayerToPlayer_TransferMode_CanonCardsTFormsController(
            Contexts contexts,
            ISingleDataProvider<TransferProcessInfo> processInfoProvider,
            IPlayersTFormsDefiner playersTFormsDefiner,
            ICameraController cam) :
            base(
                contexts, 
                processInfoProvider,
                new CalcIndexInConstRndOrder(contexts.CardIdOrder))
        {
            _cam = cam;
            _playersTFormsDefiner = playersTFormsDefiner;
        }


        protected override ITFormsCalcer NewCalcer() => new MainPlayerToPlayer_TransformMode_TFormsCalcer();


        protected override void OnNewGiverAndTaker(PlayerId giver, PlayerId taker)
        {
            _taker = taker;
        }


        protected override bool GiverHasBeenAltered() => false; //TODO: следить тут за изменениями камеры


        protected override bool TakerHasBeenAltered() => TFormHasChangedFor(_taker);


        private bool TFormHasChangedFor(PlayerId playerId) => _playersTFormsDefiner.TFormsToApply.AddedOrChanged.ContainsKey(playerId);


        protected override void ActualizeGiverInfoForCalcer(ITFormsCalcer tFormsCalcer) => ((IWithCamParamsAndPlayerTForm)tFormsCalcer).CamParams = _cam.CameraInfo.Params;


        protected override void ActualizeTakerInfoForCalcer(ITFormsCalcer tFormsCalcer) => ((IWithCamParamsAndPlayerTForm)tFormsCalcer).ChangePlayerTForm(TakerTForm);

    }
}