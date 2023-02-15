using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ���������, ������������, ����� ��������/����������� ������������ ������������ ��� ����, � ����� - ��� ����� ��������� ����.
    /// </summary>
    public abstract class CardsRelatedBeginCalcTFormsStrategy : IBeginCalcTFormsStartegy<CardId>
    {
        protected ICardsActionsProvider CardsActionsProvider { get; private set; }
        public bool ToSkipRecal => false;


        public CardsRelatedBeginCalcTFormsStrategy(ICardsActionsProvider cardsActionsProvider)
        {
            CardsActionsProvider = cardsActionsProvider;
        }


        public abstract bool IsBeginCalcTFormsEvent(out IEnumerable<CardId> objects);


        public abstract bool IsStopCalcTFormsEvent(out IEnumerable<CardId> objects);
    }
}