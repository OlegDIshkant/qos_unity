using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должны иметь карты игрока, когда тот просто держит их у себя на руках.
    /// </summary>
    public class DefiningCardsIdleTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;


        public DefiningCardsIdleTForms(
            Contexts contexts,
            ICardsActionsProvider cardsActions,
            ICursorController cursor,
            ICameraController cam,
            IPlayersTFormsDefiner playerTFormsDefiner) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var idleTFormsProviders = new Dictionary<PlayerId, IDictionaryDataProvider<CardId, Transform>>();
            foreach (var playerId in contexts.PlayersInfo.allPLayerIds)
            {
                var tForms = 
                    playerId.Equals(contexts.PlayersInfo.mainPLayerId) ?
                    (IDictionaryDataProvider<CardId, Transform>)new MainPlayer_IdleTForms(contexts, playerId, cursor, cam, cardsActions, HighlightSettingsForMainPlayer) :
                    (IDictionaryDataProvider<CardId, Transform>)new Player_IdleTForms(contexts, playerId, playerTFormsDefiner, cardsActions);
                idleTFormsProviders.Add(playerId, (IDictionaryDataProvider<CardId, Transform>)queue.AddWithoutTag((AbstractController)tForms));
            }
            var aggregator = queue.AddWithoutTag(new CardTFormsAgregator<PlayerId>(contexts, idleTFormsProviders));

            SetControllersQueue(queue);


            _finalTForms = aggregator;
        }


        private HighlightSettings HighlightSettingsForMainPlayer => new HighlightSettings(HighlightType.HEAVY, HighlightType.LIGHT);
    }


}