using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    ///  онтроллер, вычисл€ющий, какое расположение в пространстве должны иметь карты конкретного (Ќ≈ главного) игрока, когда тот просто держит их у себ€ на руках.
    /// </summary>
    public class Player_IdleTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;


        public Player_IdleTForms(
            Contexts contexts,
            PlayerId playerId,
            IPlayersTFormsDefiner playerTFormsDefiner,
            ICardsActionsProvider cardsActionsProvider) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var canonTForms = queue.AddWithoutTag(new PlayerIdleCanonCardsTFormsController(contexts, playerId, playerTFormsDefiner, cardsActionsProvider));
            var idlingCards = queue.AddWithoutTag(new Player_CardTForms_Idle_Modifier(contexts, cardsActionsProvider, canonTForms, playerId));
            var floatingCards = queue.AddWithoutTag(new Player_CardTForm_FloatAnimModifier(contexts, idlingCards));

            SetControllersQueue(queue);

            _finalTForms = floatingCards;
        }
    }
}