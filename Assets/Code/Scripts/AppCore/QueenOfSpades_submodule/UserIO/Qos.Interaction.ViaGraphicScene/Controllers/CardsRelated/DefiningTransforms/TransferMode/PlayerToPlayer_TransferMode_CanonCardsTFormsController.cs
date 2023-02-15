using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контролер, определяющий канонические положения карт игрока, когда другой игрок выбирает среди них карты для передачи себе.
    /// </summary>
    public class PlayerToPlayer_TransferMode_CanonCardsTFormsController : TransferMode_CanonTForms_Controller
    {
        private readonly IPlayersTFormsDefiner _playersTFormsDefiner;

        private PlayerId _giver;
        private PlayerId _taker;

        private Transform GiverTForm => _playersTFormsDefiner.TFormsToApply.Items[_giver];
        private Transform TakerTForm => _playersTFormsDefiner.TFormsToApply.Items[_taker];

        protected override TransferType TransferType => TransferType.FROM_PLAYER_TO_PLAYER;


        public PlayerToPlayer_TransferMode_CanonCardsTFormsController(
            Contexts contexts,
            ISingleDataProvider<TransferProcessInfo> processInfoProvider,
            IPlayersTFormsDefiner playersTFormsDefiner) :
            base(
                contexts, 
                processInfoProvider,
                new CalcIndexInConstRndOrder(contexts.CardIdOrder))
        {
            _playersTFormsDefiner = playersTFormsDefiner;
        }


        protected override ITFormsCalcer NewCalcer() => new PlayerToPlayer_TransformMode_TFormsCalcer(Transform.Default);


        protected override void OnNewGiverAndTaker(PlayerId giver, PlayerId taker)
        {
            _giver = giver;
            _taker = taker;
        }


        protected override bool GiverHasBeenAltered() => TFormHasChangedFor(_giver);


        protected override bool TakerHasBeenAltered() => TFormHasChangedFor(_taker);


        private bool TFormHasChangedFor(PlayerId playerId) => _playersTFormsDefiner.TFormsToApply.AddedOrChanged.ContainsKey(playerId);


        protected override void ActualizeGiverInfoForCalcer(ITFormsCalcer tFormsCalcer) => ((IWithTwoPlayerTForms)tFormsCalcer).ChangeFirstPlayerTForm(GiverTForm);


        protected override void ActualizeTakerInfoForCalcer(ITFormsCalcer tFormsCalcer) => ((IWithTwoPlayerTForms)tFormsCalcer).ChangeSecondPlayerTForm(TakerTForm);

    }



}