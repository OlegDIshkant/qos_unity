using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Когда другой игрок выбирает у главного игрока карты для передачи себе, этот класс
    /// визуально отображает процесс выбора, модифицируя расположения карт главного игрока.
    /// </summary>
    public class MainPlayerToPlayer_CardTForms_TransferMode_Modifier : TFormsModifier<CardId, HighlightsData>
    {
        public MainPlayerToPlayer_CardTForms_TransferMode_Modifier(
            Contexts contexts,
            IDictionaryDataProvider<CardId, Transform> canonTFormsProvider) :
            base(
                contexts,
                canonTFormsProvider,
                new AllwaysModifyStrategy(),
                new RndHighlightModifyStrategy(contexts.TimeContext))
        {
        }
    }




    /// <summary>
    /// Когда главный игрок выбирает у другого игрока карты для передачи себе, этот класс
    /// визуально отображает процесс выбора, модифицируя расположения карт другого игрока.
    /// </summary>
    public class PlayerToMainPlayer_CardTForms_TransferMode_Modifier : TFormsModifier<CardId, HighlightsData>
    {
        public PlayerToMainPlayer_CardTForms_TransferMode_Modifier(
            Contexts contexts,
            ICameraController cameraController,
            ICursorController cursorController,
            HighlightSettings highlightSettings,
            IDictionaryDataProvider<CardId, Transform> canonTFormsProvider,
            ISingleDataProvider<TransferProcessInfo> processInfoProvider) :
            base(
                contexts,
                canonTFormsProvider,
                new AllwaysModifyStrategy(),
                new SelectByCursorModifyStrategy(contexts.TimeContext, cameraController, cursorController, new PrimitiveCardsRaycaster(), 
                    new SelectCardJustOncePerTransferChoice(processInfoProvider), highlightSettings))
        {
        }
    }




    /// <summary>
    /// Когда один игрок выбирает у другого игрока карты для передачи себе, этот класс
    /// визуально отображает процесс выбора, модифицируя расположения карт первого игрока.
    /// </summary>
    public class PlayerToPlayer_CardTForms_TransferMode_Modifier : TFormsModifier<CardId, HighlightsData>
    {
        public PlayerToPlayer_CardTForms_TransferMode_Modifier(
            Contexts contexts,
            IDictionaryDataProvider<CardId, Transform> canonTFormsProvider) :
            base(
                contexts,
                canonTFormsProvider,
                new AllwaysModifyStrategy(),
                new RndHighlightModifyStrategy(contexts.TimeContext))
        {
        }
    }




}