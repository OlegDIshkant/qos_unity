using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ���������� �������������� ��������� ���� ������ ���, ����� ��������� ��������, ��� �� � ���� ��������������� (����������, �������� � ������� � ��� �����), ����� ����� �� �����.
    /// </summary>
    public class Player_CardTForms_Idle_Modifier : TFormsModifier<CardId>
    {
        public Player_CardTForms_Idle_Modifier(
            Contexts contexts, 
            ICardsActionsProvider cardsActionsProvider, 
            IDictionaryDataProvider<CardId, Transform> canonTFormsProvider, PlayerId playerId) :
            base(
                contexts,
                canonTFormsProvider,
                new ModifyWhenIdleStrategy(playerId, cardsActionsProvider),
                new RndHighlightModifyStrategy(contexts.TimeContext))
        {
        }
    }
}