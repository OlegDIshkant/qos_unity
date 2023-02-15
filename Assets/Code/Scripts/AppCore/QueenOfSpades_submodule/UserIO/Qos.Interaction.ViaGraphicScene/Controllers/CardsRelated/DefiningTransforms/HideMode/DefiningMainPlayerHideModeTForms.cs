using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    public class DefiningMainPlayerHideModeTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;


        public DefiningMainPlayerHideModeTForms(
            Contexts contexts,
            PlayerId playerId,
            ICameraController cam,
            ICardsActionsProvider cardsActionsProvider) :
            base(contexts)
        {
            var queue = new ControllersQueue();
            var canonTForms = queue.AddWithoutTag(new MainPlayer_HideMode_CanonTFormsController(contexts, playerId, cam, cardsActionsProvider));
            var smoothedTForms = queue.AddWithoutTag(new SmoothTFormTransitModifier<CardId>(contexts, 0.003f, canonTForms));
            var floatingTForms = queue.AddWithoutTag(new MainPlayer_CardTForm_FloatAnimModifier(contexts, smoothedTForms));
            SetControllersQueue(queue);

            _finalTForms = floatingTForms;
        }

    }
}