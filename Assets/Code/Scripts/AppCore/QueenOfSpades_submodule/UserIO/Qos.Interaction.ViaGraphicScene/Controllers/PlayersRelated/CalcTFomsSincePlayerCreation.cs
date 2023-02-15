using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Стратегия, определяющая, что нужно начинать считать рассположения игроков, как только один из них в игре.
    /// </summary>
    public class CalcTFomsForExpectedPlayers : IBeginCalcTFormsStartegy<PlayerId>
    {
        private readonly PlayersInfo _playersInfo;
        private readonly IHashSetDataProvider<PlayerId> _inGamePlayersProvider;

        private bool _playersCreated = false;

        public bool ToSkipRecal { get; private set; } = false;


        public CalcTFomsForExpectedPlayers(PlayersInfo playersInfo, IHashSetDataProvider<PlayerId> inGamePlayersProvider) 
        {
            _playersInfo = playersInfo;
            _inGamePlayersProvider = inGamePlayersProvider;
        }


        public bool IsBeginCalcTFormsEvent(out IEnumerable<PlayerId> objects)
        {
            if (!_playersCreated)
            {
                objects = AllExpectedPlayersAtOnce();
                _playersCreated = objects.Any();
                ToSkipRecal = _playersCreated;
                return _playersCreated;
            }

            objects = null;
            return false;
        }


        public bool IsStopCalcTFormsEvent(out IEnumerable<PlayerId> objects)
        {
            objects = null;
            return false;
        }


        private IEnumerable<PlayerId> AllExpectedPlayersAtOnce()
        {
            if (AnyPlayerStartedAppear())
            {
                return AllExpectedPlayers();
            }
            return Enumerable.Empty<PlayerId>();
        }


        private bool AnyPlayerStartedAppear() =>
            _inGamePlayersProvider.HashSetOutput?.HasChanged ?? false &&
            _inGamePlayersProvider.HashSetOutput.Added.Any();


        private IEnumerable<PlayerId> AllExpectedPlayers() => _playersInfo.otherPLayerIds;



    }
}