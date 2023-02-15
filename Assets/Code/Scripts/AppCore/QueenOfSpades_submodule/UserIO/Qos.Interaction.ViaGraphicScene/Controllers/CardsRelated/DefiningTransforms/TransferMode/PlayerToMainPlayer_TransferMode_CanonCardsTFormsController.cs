using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{

    /// <summary>
    /// Контролер, определяющий канонические положения карт игрока, когда главный игрок (пользователь) выбирает среди них карты для передачи себе.
    /// </summary>
    public class PlayerToMainPlayer_TransferMode_CanonCardsTFormsController : TransferMode_CanonTForms_Controller
    {
        private readonly IPlayersTFormsDefiner _playersTFormsDefiner;
        private readonly ICameraController _cam;

        private PlayerId _giver;

        private Transform GiverTForm => _playersTFormsDefiner.TFormsToApply.Items[_giver];

        protected override TransferType TransferType => TransferType.FROM_PLAYER_TO_MAIN_PLAYER;


        public PlayerToMainPlayer_TransferMode_CanonCardsTFormsController(
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


        protected override ITFormsCalcer NewCalcer() => new PlayerToMainPlayer_TransformMode_TFormsCalcer();


        protected override void OnNewGiverAndTaker(PlayerId giver, PlayerId taker)
        {
            _giver = giver;
        }


        protected override bool GiverHasBeenAltered() => TFormHasChangedFor(_giver);


        protected override bool TakerHasBeenAltered() => false; // TODO: проверять камеру на изменение


        private bool TFormHasChangedFor(PlayerId playerId) => _playersTFormsDefiner.TFormsToApply.AddedOrChanged.ContainsKey(playerId);


        protected override void ActualizeGiverInfoForCalcer(ITFormsCalcer tFormsCalcer) => ((IWithCamParamsAndPlayerTForm)tFormsCalcer).ChangePlayerTForm(GiverTForm);


        protected override void ActualizeTakerInfoForCalcer(ITFormsCalcer tFormsCalcer) => ((IWithCamParamsAndPlayerTForm)tFormsCalcer).CamParams = _cam.CameraInfo.Params;

    }









}