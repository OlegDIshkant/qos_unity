using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// –еализаци€ контроллера, определ€ющего канонические положени€ карт игрока дл€ определеенной ситуации.
    /// </summary>
    public sealed class PlayerCanonCardsTFormsControllerImpl : CanonTFormsControllerImpl<CardId>
    {
        private readonly PlayerId _playerId;
        private readonly IPlayersTFormsDefiner _playerTFormsDefiner;
        private readonly IWithPlayerTForm _tFormsCalcer;


        public PlayerCanonCardsTFormsControllerImpl(
            PlayerId playerId,
            IPlayersTFormsDefiner playerTFormsDefiner,
            IWithPlayerTForm tFormsCalcer,
            IBeginCalcTFormsStartegy<CardId> beginCalcStrategy)
            :
            base(beginCalcStrategy, tFormsCalcer)
        {
            _playerId = playerId;
            _playerTFormsDefiner = playerTFormsDefiner;
            _tFormsCalcer = tFormsCalcer;

            if (_playerTFormsDefiner.TFormsToApply?.Items.TryGetValue(_playerId, out var tForm) ?? false)
            {
                ConsiderChangedPlayerTForm();
            }
        }


        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();
            ActualizePlayerTransform();
        }


        private void ActualizePlayerTransform()
        {
            if (_playerTFormsDefiner.TFormsToApply.HasChanged && 
                _playerTFormsDefiner.TFormsToApply.AddedOrChanged.ContainsKey(_playerId))
            {
                ConsiderChangedPlayerTForm();
            }
        }

        private void ConsiderChangedPlayerTForm()
        {
            if (_playerTFormsDefiner.TFormsToApply.Items.TryGetValue(_playerId, out var tForm))
            {
                _tFormsCalcer.ChangePlayerTForm(tForm);
            }
        }


    }
}