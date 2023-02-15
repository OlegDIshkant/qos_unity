using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должны иметь карты игрока, когда тот выбирает среди них те, что собирается сбросить.
    /// </summary>
    public class Player_DiscModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {

        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;


        public Player_DiscModeTForms(
            Contexts contexts,
            PlayerId playerId,
            IPlayersTFormsDefiner playersTFormsDefiner,
            ICardsActionsProvider cardsActionsProvider) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var canonTForms = queue.AddWithoutTag(new PlayerDiscModeCanonCardsTFormsController(contexts, playersTFormsDefiner, cardsActionsProvider, playerId));
            var highlinghtedTForms = queue.AddWithoutTag(new Player_CardTForms_DiscMode_Modifier(contexts, canonTForms, playerId, cardsActionsProvider));
            var floatingTForms = queue.AddWithoutTag(new Player_CardTForm_FloatAnimModifier(contexts, highlinghtedTForms));

            SetControllersQueue(queue);

            _finalTForms = floatingTForms;
        }
    }
}