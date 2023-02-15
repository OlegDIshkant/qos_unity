using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Контроллер, считающий канонические положения игроков.
    /// </summary>
    internal class CanonPlayerTFormsCalcer : CanonTFormsController<PlayerId>
    {
        public CanonPlayerTFormsCalcer(
            Contexts contexts,
            IPlayFieldController playField,
            IHashSetDataProvider<PlayerId> inGamePlayersProvider) :
            base(
                contexts,
                new CanonTFormsControllerImpl<PlayerId>(
                    new CalcTFomsForExpectedPlayers(contexts.PlayersInfo, inGamePlayersProvider),
                    new PlayersTransformCalculator(playField)),
                new CalcIndexInPlayersOrder(contexts.PlayersInfo.allPLayerIds, contexts.PlayersInfo.mainPLayerId))
        {
        }

    }
}