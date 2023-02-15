using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ћодифицирует положени€ карт главного игрока во врем€ режима выбора карт дл€ сброса,
    /// визуально отобража€ взаимодействие карт с курсором пользовател€.
    /// </summary>
    public class MainPlayer_CardTForms_DiscMode_ByCursor_Modifier : TFormsModifier<CardId, HighlightsData>
    {
        public MainPlayer_CardTForms_DiscMode_ByCursor_Modifier(
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
                new ModifyWhenDiscModeStrategy(playerId, cardsActionsProvider),
                new SelectByCursorModifyStrategy(contexts.TimeContext, cameraController, cursorController, new PrimitiveCardsRaycaster(), new FreelySelectCards(), highlightSettings))
        {
        }
    }
}