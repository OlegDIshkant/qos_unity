using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Контроллер, сообщающий какие игроки в данный момент должны существовать на игровом поле.
    /// </summary>
    public class InGamePlayersProviders : AbstractController, IHashSetDataProvider<PlayerId>
    {
        private readonly IPlayerActionsProvider _playersActionsProvider;
        private readonly PlayerId _mainPlayerId;

        private ListData<PlayerId>.Editable _output;
        public ListData<PlayerId> HashSetOutput { get; private set; }


        public InGamePlayersProviders(
            Contexts contexts,
            PlayerId mainPlayerId,
            IPlayerActionsProvider playersActionsProvider) : 
            base(contexts)
        {
            _mainPlayerId = mainPlayerId;
            _playersActionsProvider = playersActionsProvider;

            HashSetOutput = new ListData<PlayerId>(out _output);
        }


        public override void Update()
        {
            foreach (var player in PlayersStartedBeingCreated())
            {
                RememberPlayer(player);
            }


            foreach (var player in PlayersLeftMatch())
            {
                ForgetPlayer(player);
            }
        }


        private IEnumerable<PlayerId> PlayersStartedBeingCreated()
        {
            if (_playersActionsProvider.DictionaryOutput.HasChanged)
            {
                foreach (var item in _playersActionsProvider.DictionaryOutput.AddedOrChanged)
                {
                    if (!item.Key.Equals(_mainPlayerId) && item.Value.IsBeginingOfPlayerCreation)
                    {
                        yield return item.Key;
                    }
                }
            }
        }


        private void RememberPlayer(PlayerId playerId) => _output.AddItem(playerId);


        private void ForgetPlayer(PlayerId playerId) => _output.RemoveItem(playerId);



        private IEnumerable<PlayerId> PlayersLeftMatch()
        {
            if (_playersActionsProvider.DictionaryOutput.HasChanged)
            {
                foreach (var item in _playersActionsProvider.DictionaryOutput.AddedOrChanged)
                {
                    if (!item.Key.Equals(_mainPlayerId) && item.Value.IsOutOfGame())
                    {
                        yield return item.Key;
                    }
                }
            }
        }
    }
}