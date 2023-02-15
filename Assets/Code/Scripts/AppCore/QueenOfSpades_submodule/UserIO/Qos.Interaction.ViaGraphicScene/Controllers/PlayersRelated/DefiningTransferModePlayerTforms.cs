using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using System.Collections.Generic;
using CommonTools;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers
{
    /// <summary>
    /// Контроллер, определяющий положения тех игроков, что в данный момент учавствуют в процессе передачи карт от одного к другому.
    /// </summary>
    public class DefiningTransferModePlayerTforms : AbstractController, IDictionaryDataProvider<PlayerId, Transform>
    {
        private readonly IPlayerActionsProvider _playersActionsProvider;
        private readonly IDictionaryDataProvider<PlayerId, Transform> _canonPlayerTFormsProvider;
        private readonly ICameraController _cam;

        private HashSet<PlayerId> _knownPlayers = new HashSet<PlayerId>();

        private DictionaryData<PlayerId, Transform>.Editable _output;
        public DictionaryData<PlayerId, Transform> DictionaryOutput { get; private set; }

        private PlayerId MainPlayerId => Contexts.PlayersInfo.mainPLayerId;


        public DefiningTransferModePlayerTforms(
            Contexts contexts,
            ICameraController cam,
            IDictionaryDataProvider<PlayerId, Transform> canonPlayerTFormsProvider,
            IPlayerActionsProvider playersActionsProvider) :
            base(contexts)
        {
            _cam = cam;
            _playersActionsProvider = playersActionsProvider;
            _canonPlayerTFormsProvider = canonPlayerTFormsProvider;

            DictionaryOutput = new DictionaryData<PlayerId, Transform>(out _output);
        }


        public override void Update()
        {
            CheckOutCanonTForms();
            ModifyCanonTFormsIfNeeded();
            ForgetCardsIfNeeded();
        }


        private void CheckOutCanonTForms()
        {
            if (_canonPlayerTFormsProvider.DictionaryOutput.HasChanged)
            {
                foreach (var item in _canonPlayerTFormsProvider.DictionaryOutput.AddedOrChanged) _output.SetItem(item.Key, item.Value);
                foreach (var item in _canonPlayerTFormsProvider.DictionaryOutput.Removed) _output.RemoveItem(item);
            }
        }


        private void ModifyCanonTFormsIfNeeded()
        {
            foreach (var (playerA, playerB) in NewPlayersInTransferMode())
            {
                Logger.Verbose($"Пересчитываем положения для игроков: '{playerA}' and '{playerB}'.");
                RecalcTforms(playerA, playerB);
            }
        }


        private IEnumerable<(PlayerId, PlayerId)> NewPlayersInTransferMode()
        {
            foreach (var (playerA, playerB) in UnknownPlayersInTransferMode())
            {
                RememberPlayers(playerA, playerB);
                yield return (playerA, playerB);
            }
        }


        private IEnumerable<(PlayerId, PlayerId)> UnknownPlayersInTransferMode()
        {
            if (_playersActionsProvider.DictionaryOutput.HasChanged)
            {
                foreach (var item in _playersActionsProvider.DictionaryOutput.AddedOrChanged)
                {
                    if (item.Value.IsGoingToTransferMode(out var otherPlayer) &&
                        item.Value.NormTime == NormValue.Min &&
                        !_knownPlayers.Contains(item.Key))
                    {
                        yield return (item.Key, otherPlayer);
                    }
                }
            }
        }


        private void RememberPlayers(PlayerId playerA, PlayerId playerB)
        {
            _knownPlayers.Add(playerA);
            _knownPlayers.Add(playerB);
        }


        private void ForgetPlayers(PlayerId playerA, PlayerId playerB)
        {
            _knownPlayers.Remove(playerA);
            _knownPlayers.Remove(playerB);

            SetPlayerTForm(playerA, GetCannonPlayerTForm(playerA));
            SetPlayerTForm(playerB, GetCannonPlayerTForm(playerB));
        }


        private void SetPlayerTForm(PlayerId player, Transform tForm)
        {
            if (!player.Equals(MainPlayerId))
            {
                _output.SetItem(player, tForm);
            }
        }


        private void RecalcTforms(PlayerId toChangeA, PlayerId toChangeB)
        {
            var (newTFormA, newTFormB) = MakeLookAtEachOther(GetCannonPlayerTForm(toChangeA), GetCannonPlayerTForm(toChangeB));
            SetPlayerTForm(toChangeA, newTFormA);
            SetPlayerTForm(toChangeB, newTFormB);
        }


        private Transform GetCannonPlayerTForm(PlayerId player) =>
            player.Equals(MainPlayerId) ? _cam.CameraInfo.GetTransform() : DictionaryOutput.Items[player];


        private (Transform, Transform) MakeLookAtEachOther(Transform a, Transform b)
        {
            var a2b = b.Position - a.Position;
            a2b.Y = 0; 
            a.Rotation = GeometryUtils.InitialRotation(a2b);
            b.Rotation = GeometryUtils.InitialRotation(-a2b);
            return (a, b);
        }



        private void ForgetCardsIfNeeded()
        {
            foreach (var (playerA, playerB) in KnownPlayersLeftTransferMode())
            {
                ForgetPlayers(playerA, playerB);
            }
        }


        private IEnumerable<(PlayerId, PlayerId)> KnownPlayersLeftTransferMode()
        {
            if (_playersActionsProvider.DictionaryOutput.HasChanged)
            {
                foreach (var item in _playersActionsProvider.DictionaryOutput.AddedOrChanged)
                {
                    if (item.Value.IsGoingOutTransferMode(out var otherPlayer) &&
                        item.Value.NormTime == NormValue.Max &&
                        _knownPlayers.Contains(item.Key))
                    {
                        yield return (item.Key, otherPlayer);
                    }
                }
            }
        }
    }
}