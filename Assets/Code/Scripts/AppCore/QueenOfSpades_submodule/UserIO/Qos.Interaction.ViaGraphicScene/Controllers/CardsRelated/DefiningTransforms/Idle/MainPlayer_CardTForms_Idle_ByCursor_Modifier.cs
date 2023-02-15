using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Когда главный игрок (пользователь) отдыхает, этот контроллер модифицирует положения его карт так, чтобы вызуально отобразить их взаимодействие с курсором.
    /// </summary>
    public class MainPlayer_CardTForms_Idle_ByCursor_Modifier : TFormsModifier<CardId>
    {
        public MainPlayer_CardTForms_Idle_ByCursor_Modifier(
            Contexts contexts,
            ICameraController cameraController,
            ICursorController cursorController,
            IDictionaryDataProvider<CardId, Transform> canonTFormsProvider,
            PlayerId playerId,
            ICardsActionsProvider cardsActionsProvider,
            HighlightSettings highlightSettings) :
            base(
                contexts,
                canonTFormsProvider,
                new ModifyWhenIdleStrategy(playerId, cardsActionsProvider),
                new SelectByCursorModifyStrategy(contexts.TimeContext, cameraController, cursorController, new PrimitiveCardsRaycaster(), new NeverSelectCards(), highlightSettings))
        {
        }
    }
}