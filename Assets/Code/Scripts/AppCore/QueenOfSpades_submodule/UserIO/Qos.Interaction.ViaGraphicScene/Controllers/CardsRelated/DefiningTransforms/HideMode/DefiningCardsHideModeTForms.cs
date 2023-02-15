using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должны иметь карты игрока, когда тот выбирает среди среди карт другого игрока те, что заберет себе.
    /// </summary>
    public class DefiningCardsHideModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;


        public DefiningCardsHideModeTForms(
            Contexts contexts,
            ICardsActionsProvider cardsActions,
            ICursorController cursor,
            ICameraController cam,
            IPlayersTFormsDefiner playerTFormsDefiner) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var hideTFormsProviders = new Dictionary<PlayerId, IDictionaryDataProvider<CardId, Transform>>();
            foreach (var playerId in contexts.PlayersInfo.allPLayerIds)
            {
                var tForms =
                    playerId.Equals(contexts.PlayersInfo.mainPLayerId) ?
                    (IDictionaryDataProvider<CardId, Transform>)new DefiningMainPlayerHideModeTForms(contexts, playerId, cam, cardsActions) :
                    (IDictionaryDataProvider<CardId, Transform>)new DefiningPlayerHideModeTForms(contexts, playerId, cardsActions, playerTFormsDefiner);
                hideTFormsProviders.Add(playerId, (IDictionaryDataProvider<CardId, Transform>)queue.AddWithoutTag((AbstractController)tForms));
            }
            var aggregator = queue.AddWithoutTag(new CardTFormsAgregator<PlayerId>(contexts, hideTFormsProviders));

            SetControllersQueue(queue);


            _finalTForms = aggregator;
        }


    }
}