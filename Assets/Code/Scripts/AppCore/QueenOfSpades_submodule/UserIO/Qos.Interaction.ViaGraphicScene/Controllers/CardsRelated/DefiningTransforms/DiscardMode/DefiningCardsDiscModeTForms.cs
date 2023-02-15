using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должны иметь карты игрока, когда тот выбирает, какие карты сбросить.
    /// </summary>
    public class DefiningCardsDiscModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>, IHashSetDataProvider<CardId>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTFormsProvider;
        private readonly IHashSetDataProvider<CardId> _cardsToDiscardProvider;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTFormsProvider.DictionaryOutput;
        public ListData<CardId> HashSetOutput => _cardsToDiscardProvider.HashSetOutput;


        public DefiningCardsDiscModeTForms(
            Contexts contexts,
            ICardsActionsProvider cardsActions,
            ICursorController cursor,
            ICameraController cam,
            IPlayersTFormsDefiner playerTFormsDefiner) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var discModeTFormsProviders = new Dictionary<PlayerId, IDictionaryDataProvider<CardId, Transform>>();
            foreach (var playerId in contexts.PlayersInfo.allPLayerIds)
            {
                var tForms =
                    playerId.Equals(contexts.PlayersInfo.mainPLayerId) ?
                    (IDictionaryDataProvider<CardId, Transform>)new MainPlayer_DiscModeTForms(contexts, playerId, cursor, cam, cardsActions, HighlightSettingsForMainPlayer) :
                    (IDictionaryDataProvider<CardId, Transform>)new Player_DiscModeTForms(contexts,  playerId, playerTFormsDefiner, cardsActions);
                discModeTFormsProviders.Add(playerId, (IDictionaryDataProvider<CardId, Transform>)queue.AddWithoutTag((AbstractController)tForms));
            }
            var aggregator = queue.AddWithoutTag(new CardDiscModeTFormsAgregator(contexts, cardsActions, discModeTFormsProviders));

            SetControllersQueue(queue);


            _finalTFormsProvider = aggregator;
            _cardsToDiscardProvider = (IHashSetDataProvider<CardId>)discModeTFormsProviders[contexts.PlayersInfo.mainPLayerId];
        }


        private HighlightSettings HighlightSettingsForMainPlayer => new HighlightSettings(HighlightType.HEAVY, HighlightType.LIGHT);
    }
}