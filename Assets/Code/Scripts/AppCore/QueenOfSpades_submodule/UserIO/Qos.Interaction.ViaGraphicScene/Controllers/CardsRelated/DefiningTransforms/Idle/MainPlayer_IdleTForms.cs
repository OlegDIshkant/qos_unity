using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должны иметь карты главного игрока (пользователь), когда тот просто держит их у себя на руках.
    /// </summary>
    public class MainPlayer_IdleTForms : ControllersController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IDictionaryDataProvider<CardId, Transform> _finalTForms;

        public DictionaryData<CardId, Transform> DictionaryOutput => _finalTForms.DictionaryOutput;


        public MainPlayer_IdleTForms(
            Contexts contexts,
            PlayerId mainPlayerId,
            ICursorController cursor,
            ICameraController cam,
            ICardsActionsProvider cardsActionsProvider,
            HighlightSettings highlightSettings) :
            base(contexts)
        {
            var queue = new ControllersQueue();

            var canonTForms = queue.AddWithoutTag(new MainPlayerIdleCanonCardsTFormsController(contexts, mainPlayerId, cam, cardsActionsProvider));
            var modByCursorTForms = queue.AddWithoutTag(new MainPlayer_CardTForms_Idle_ByCursor_Modifier(contexts, cam, cursor, canonTForms, mainPlayerId, cardsActionsProvider, highlightSettings));
            var smoothedTForms = queue.AddWithoutTag(new SmoothTFormTransitModifier<CardId>(contexts, 0.003f, modByCursorTForms));
            var floatingTForms = queue.AddWithoutTag(new MainPlayer_CardTForm_FloatAnimModifier(contexts, smoothedTForms));

            SetControllersQueue(queue);

            _finalTForms = floatingTForms;
        }
    }




}