using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Контроллер, определяющий положения тех игроков, что в данный момент учавствуют в процессе передачи карт от одного к другому.
    /// </summary>
    public class DefiningTransitPlayerTForms : AbstractController, IDictionaryDataProvider<PlayerId, Transform>
    {
        private readonly IPlayerActionsProvider _playersActionsProvider;
        private readonly IDictionaryDataProvider<PlayerId, Transform> _canonPlayerTFormsProvider;
        private readonly IDictionaryDataProvider<PlayerId, Transform> _transferModePlayerTFormsProvider;
        private readonly IDictionaryDataProvider<PlayerId, Transform> _prevPlayerTFormsProvider;

        private Dictionary<PlayerId, Transform> _initialTForms = new Dictionary<PlayerId, Transform>();

        private DictionaryData<PlayerId, Transform>.Editable _output;
        public DictionaryData<PlayerId, Transform> DictionaryOutput { get; private set; }


        public DefiningTransitPlayerTForms(
            Contexts contexts,
            IPlayerActionsProvider playersActionsProvider,
            IDictionaryDataProvider<PlayerId, Transform> canonPlayerTFormsProvider,
            IDictionaryDataProvider<PlayerId, Transform> transferModePlayerTFormsProvider,
            IDictionaryDataProvider<PlayerId, Transform> prevPlayerTFormsProvider) :
            base(contexts)
        {
            _playersActionsProvider = playersActionsProvider;
            _canonPlayerTFormsProvider = canonPlayerTFormsProvider;
            _transferModePlayerTFormsProvider = transferModePlayerTFormsProvider;
            _prevPlayerTFormsProvider = prevPlayerTFormsProvider;

            DictionaryOutput = new DictionaryData<PlayerId, Transform>(out _output);
        }


        public override void Update()
        {
            foreach (var (player, action) in ChangedPlayerAction())
            {
                TryUpdateTForm(player, action);
            }
        }


        private IEnumerable<(PlayerId, PlayerAction)> ChangedPlayerAction()
        {
            if (_playersActionsProvider.DictionaryOutput.HasChanged)
            {
                foreach (var item in _playersActionsProvider.DictionaryOutput.AddedOrChanged)
                {
                    if (item.Key.Equals(Contexts.PlayersInfo.mainPLayerId)) continue;
                    yield return (item.Key, item.Value);
                }
            }
        }


        private void TryUpdateTForm(PlayerId player, PlayerAction action)
        {
            if (action.IsGoingToTransferMode(out var _))
            {
                ChangeTForm(player, action, targetTForm: _transferModePlayerTFormsProvider.DictionaryOutput.Items[player]);
            }
            else if (action.IsGoingOutTransferMode(out var _))
            {
                ChangeTForm(player, action, targetTForm: _canonPlayerTFormsProvider.DictionaryOutput.Items[player]);
            }
        }


        private void ChangeTForm(PlayerId player, PlayerAction action, Transform targetTForm)
        {
            if (action.NormTime == NormValue.Min)
            {
                _initialTForms[player] = _prevPlayerTFormsProvider.DictionaryOutput.Items[player];
            }

            var newTform = Transform.Lerp(_initialTForms[player], targetTForm, action.NormTime.AsFloat);
            _output.SetItem(player, newTform);
        }
    }
}