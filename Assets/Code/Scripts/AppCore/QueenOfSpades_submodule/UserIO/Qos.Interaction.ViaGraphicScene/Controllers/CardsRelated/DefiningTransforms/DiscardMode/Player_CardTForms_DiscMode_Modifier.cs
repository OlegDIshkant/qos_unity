using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    ///  онтроллер, который должен модифицировать положени€ карт так, чтобы визуально изобразить, как игрок выбирает среди них те, что будет пытатьс€ сбросить.
    /// </summary>
    public class Player_CardTForms_DiscMode_Modifier : TFormsModifier<CardId>
    {
        public Player_CardTForms_DiscMode_Modifier(
            Contexts contexts, 
            IDictionaryDataProvider<CardId, Transform> canonTFormsProvider, 
            PlayerId playerId,
            ICardsActionsProvider cardsActionsProvider) :
            base(
                contexts,
                canonTFormsProvider,
                new ModifyWhenDiscModeStrategy(playerId, cardsActionsProvider),
                new RndHighlightModifyStrategy(contexts.TimeContext))
        {
        }
    }
}