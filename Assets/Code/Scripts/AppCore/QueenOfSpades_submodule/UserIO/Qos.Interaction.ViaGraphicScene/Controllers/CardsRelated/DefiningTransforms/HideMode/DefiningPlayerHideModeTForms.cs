using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    public class DefiningPlayerHideModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;


        public DefiningPlayerHideModeTForms(
            Contexts contexts,
            PlayerId playerId,
            ICardsActionsProvider cardsActionsProvider,
            IPlayersTFormsDefiner playerTFormsDefiner) :
            base(contexts)
        {
            var queue = new ControllersQueue();
            var canonTForms = queue.AddWithoutTag(new Player_HideMode_CanonTFormsController(contexts, playerId, playerTFormsDefiner, cardsActionsProvider));
            var smoothedTForms = queue.AddWithoutTag(new SmoothTFormTransitModifier<CardId>(contexts, 0.003f, canonTForms));
            var floatingTForms = queue.AddWithoutTag(new Player_CardTForm_FloatAnimModifier(contexts, smoothedTForms));
            SetControllersQueue(queue);

            _finalTForms = floatingTForms;
        }

    }
}