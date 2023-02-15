using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects
{
    public interface IPlayersController : ITFormsProvider<PlayerId>
    {
        ImmutableDictionary<PlayerId, IPlayer_ReadOnly> PlayersInfo { get; }
    }


    public interface ITFormsProvider<K>
    {
        event Action<K, Transform> OnTransformChanged;
        ImmutableDictionary<K, ITransformReadOnly> TransformInfo { get; }
    }


    /// <summary>
    /// Контроллер, управляющий игроками.
    /// </summary>
    public class PlayersController : AbstractController, IPlayersController
    {
        public event Action<PlayerId, Transform> OnTransformChanged;

        private readonly Func<PlayerModel, IPlayer> _PlayerFactoryMethod;
        private readonly IPlayersTFormsDefiner _tFormProvider;
        private readonly IHashSetDataProvider<PlayerId> _inGamePlayers;

        private Dictionary<PlayerId, IPlayer> _players;

        private DictionaryData<PlayerId, Transform>.Editable _dataEdit;


        public DictionaryData<PlayerId, Transform> DictionaryOutput { get; private set; }
        public TimeContext TimeContext { set; private get; }


        public ImmutableDictionary<PlayerId, IPlayer_ReadOnly> PlayersInfo => throw new NotImplementedException();

        public ImmutableDictionary<PlayerId, ITransformReadOnly> TransformInfo => throw new NotImplementedException();

        public PlayersController(
            Contexts contexts,
            Func<PlayerModel, IPlayer> PlayerFactoryMethod,
            IPlayersTFormsDefiner tFormDefiner,
            IHashSetDataProvider<PlayerId> inGamePlayers) :
            base (contexts)
        { 
            _PlayerFactoryMethod = PlayerFactoryMethod;
            _tFormProvider = tFormDefiner;
            _inGamePlayers = inGamePlayers;

            DictionaryOutput = new DictionaryData<PlayerId, Transform>(out _dataEdit);

            _players = new Dictionary<PlayerId, IPlayer>();
        }


        public override void Update()
        {
            UpdatePlayersExistance();
            UpdatePlayersTForms();
        }


        private void UpdatePlayersExistance()
        {
            if (_inGamePlayers.HashSetOutput.HasChanged)
            {
                foreach (var toSpawn in _inGamePlayers.HashSetOutput.Added)
                {
                    SpawnPlayer(toSpawn);
                }
                foreach (var toDestroy in _inGamePlayers.HashSetOutput.Removed)
                {
                    DestroyPlayer(toDestroy);
                }
            }
        }


        private void UpdatePlayersTForms()
        {
            foreach (var (player, tForm) in ChangedPlayersTForms())
            {
                ApplyNewTFormForPlayer(player, tForm);
            }
        }


        private void SpawnPlayer(PlayerId playerId)
        {
            var newPlayer = _PlayerFactoryMethod(default);
            _players[playerId] = newPlayer;
        }


        private void DestroyPlayer(PlayerId playerId)
        {
            if (_players.TryGetValue(playerId, out var player))
            {
                Logger.Verbose($"Удаляем игрока '{playerId}'.");
                player.Dispose();
                _players.Remove(playerId);
            }
            else
            {
                Logger.Error($"Не можем удалит игрока '{playerId}', так как его нет в списке активных игроков.");
            }
        }


        private IEnumerable<(PlayerId, Transform)> ChangedPlayersTForms()
        {
            if (_tFormProvider?.TFormsToApply?.HasChanged ?? false)
            {
                foreach (var item in _tFormProvider.TFormsToApply.AddedOrChanged)
                {
                    yield return (item.Key, item.Value);
                }
            }
        }


        private void ApplyNewTFormForPlayer(PlayerId playerId, Transform tForm)
        {
            if (_players.TryGetValue(playerId, out var player))
            {
                Logger.Verbose($"Назначем игроку '{playerId}' новое расположение в пространстве '{tForm}'.");
                _players[playerId].SetTransform(tForm);
                OnTransformChanged?.Invoke(playerId, tForm);
            }
            else
            {
                Logger.Error($"Не можем назначем игроку '{playerId}' новое расположение в пространстве, так как его нет в списке активных игроков.");
            }
        }




    }




}





