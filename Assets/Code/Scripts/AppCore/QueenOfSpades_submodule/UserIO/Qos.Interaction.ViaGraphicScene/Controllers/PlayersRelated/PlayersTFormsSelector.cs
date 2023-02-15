using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using CommonTools.Math;
using System.Collections.Generic;
using CommonTools;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Контроллер, который подытоживает результаты работы нескольких разных определителей положений игроков,
    /// составляя список актуальных положений игроков в данный момент времени.
    /// </summary>
    public class PlayersTFormsSelector : AbstractController, IDictionaryDataProvider<PlayerId, Transform>
    {
        private readonly IPlayerActionsProvider _playersActionsProvider;
        private readonly IDictionaryDataProvider<PlayerId, Transform> _canonPlayerTFormsProvider;
        private readonly IDictionaryDataProvider<PlayerId, Transform> _transferModePlayerTFormsProvider;
        private readonly IDictionaryDataProvider<PlayerId, Transform> _transitTFormsProvider;
        private readonly PlayerId _mainPlayerId;

        private Dictionary<PlayerId, Func<Transform>> _ToCalcTForm = new Dictionary<PlayerId, Func<Transform>>();


        public DictionaryData<PlayerId, Transform>.Editable _outputEdit;
        public DictionaryData<PlayerId, Transform> DictionaryOutput { get; private set; }


        public PlayersTFormsSelector(
            Contexts contexts,
            IPlayerActionsProvider playersActionsProvider,
            IDictionaryDataProvider<PlayerId, Transform> canonPlayerTFormsProvider,
            IDictionaryDataProvider<PlayerId, Transform> transferModePlayerTFormsProvider,
            IDictionaryDataProvider<PlayerId, Transform> transitTFormsProvider) :
            base(contexts)
        {
            _playersActionsProvider = playersActionsProvider;
            _canonPlayerTFormsProvider = canonPlayerTFormsProvider;
            _transferModePlayerTFormsProvider = transferModePlayerTFormsProvider;
            _transitTFormsProvider = transitTFormsProvider;

            DictionaryOutput = new DictionaryData<PlayerId, Transform>(out _outputEdit);
            _mainPlayerId = contexts.PlayersInfo.mainPLayerId;
        }


        public override void Update()
        {
            UpdateSources();
            UseSourcesToUpdateTForms();
        }


        private void UpdateSources()
        {
            if (ShouldUpdateSources())
            {
                foreach (var (player, action) in ChangedPlayersActions())
                {
                    UpdateSource(player, action);
                }
            }
        }


        private bool ShouldUpdateSources() => _playersActionsProvider.DictionaryOutput.HasChanged;


        private IEnumerable<(PlayerId, PlayerAction)> ChangedPlayersActions()
        {
            foreach (var changed in _playersActionsProvider.DictionaryOutput.AddedOrChanged)
            {
                if (IsMainPlayer(changed.Key)) continue;
                yield return (changed.Key, changed.Value);
            }

            bool IsMainPlayer(PlayerId player) => player.Equals(_mainPlayerId); 
        }


        private void UpdateSource(PlayerId player, PlayerAction action)
        {
            if (ForgetCardIfNeeded(player, action))
            {
                return;
            }
            ChangeSourceIfNeeded(player, action);
        }


        private bool ForgetCardIfNeeded(PlayerId player, PlayerAction action)
        {
            if (action.IsOutOfGame())
            {
                _ToCalcTForm.Remove(player);
                return true;
            }
            return false;
        }


        private void ChangeSourceIfNeeded(PlayerId player, PlayerAction action)
        {
            Func<Transform> newSource = action.Type switch
            {
                PlayerAction.Types.CREATING when action.NormTime == NormValue.Min => () => GetCanonTForm(player),
                PlayerAction.Types.IDLE => () => GetCanonTForm(player),
                PlayerAction.Types.IN_TRANSFER_MODE => () => GetTransferTForm(player),
                PlayerAction.Types.GOING_TO_TRANSFER_MODE when  action.NormTime == NormValue.Min => () => GetTransitTForm(player),
                PlayerAction.Types.GOING_OUT_TRANSFER_MODE when action.NormTime == NormValue.Min => () => GetTransitTForm(player),

                _ => null
            };

            if (newSource != null)
            {
                _ToCalcTForm[player] = newSource;
            }
        }


        private Transform GetCanonTForm(PlayerId player)
        {
            if (_canonPlayerTFormsProvider?.DictionaryOutput?.Items != null &&
                _canonPlayerTFormsProvider.DictionaryOutput.Items.TryGetValue(player, out var tform))
            {
                return tform;
            }
            return Transform.Default;
        }


        private Transform GetTransferTForm(PlayerId player)
        {
            if (_transferModePlayerTFormsProvider?.DictionaryOutput?.Items != null &&
                _transferModePlayerTFormsProvider.DictionaryOutput.Items.TryGetValue(player, out var tform))
            {
                return tform;
            }
            return Transform.Default;
        }


        private Transform GetTransitTForm(PlayerId player)
        {
            if (_transitTFormsProvider?.DictionaryOutput?.Items != null &&
                _transitTFormsProvider.DictionaryOutput.Items.TryGetValue(player, out var tform))
            {
                return tform;
            }
            return Transform.Default;
        }


        private void UseSourcesToUpdateTForms()
        {
            foreach (var item in _ToCalcTForm)
            {
                _outputEdit.SetItem(item.Key, item.Value());
            }
        }

    }
}