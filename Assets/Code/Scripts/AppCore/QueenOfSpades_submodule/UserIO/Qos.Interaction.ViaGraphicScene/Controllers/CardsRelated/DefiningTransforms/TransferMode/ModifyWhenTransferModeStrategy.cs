using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ���������, ������� ������ �������������� ��������� ���� �� �����, ���� � ������, ���������� �������, 
    /// �����-���� ������ ����� �������� ����� ��� �������� ����.
    /// </summary>
    public class ModifyWhenTransferModeStrategy : SwitchableWhenToModifyStategy
    {
        /// <summary>
        /// True - �������, ��� ������� ������ �������, ���� �� ������� ��� ������, ��������� ����� (<see cref="_giverId"/>).
        /// False - �������, ��� ������� ������ �������, ���� �� ������� ��� ������, �������� ����� (<see cref="_takerId"/>).
        /// </summary>
        private readonly bool _takerIsFixed;

        private PlayerId _giverId;
        private PlayerId? _takerIdToIgnore; // ��� ��������� ������� ����� ����������� ����������� ��������� ������ ������ 
        private PlayerId _takerId;


        /// <param name="takerIdToIgnore"> ��������� �� ������������ �����, ���� ����� c ���� id �������� ����� ����� ���� <paramref name="giverId"/>. </param>
        public ModifyWhenTransferModeStrategy(PlayerId giverId, PlayerId? takerIdToIgnore)
        {
            _takerIsFixed = false;
            _giverId = giverId;
            _takerIdToIgnore = takerIdToIgnore;
        }


        public ModifyWhenTransferModeStrategy(PlayerId takerId)
        {
            _takerIsFixed = true;
            _takerId = takerId;
        }


        protected override bool IfEnterModifiesAllowedState()
        {
            return false; // TransferModeStarted(@event);
        }


        private bool TransferModeStarted(IEvent @event)
        {
            if (_takerIsFixed)
            {
                return @event is PlayersGoingToTrasferModeEvent ev && ev.CardTakerId.Equals(_takerId);
            }
            else
            {
                return @event is PlayersGoingToTrasferModeEvent ev && ev.CardGiverId.Equals(_giverId) && IsAllowedTaker(ev.CardTakerId);
            }
        }


        private bool IsAllowedTaker(PlayerId takerId) => _takerIdToIgnore == null || !_takerIdToIgnore.Equals(takerId);


        protected override bool IfExitModifiesAllowedState()
        {
            return false;// TransferModeFinished(@event);
        }


        private bool TransferModeFinished(IEvent @event)
        {
            if (_takerIsFixed)
            {
                return @event is PlayersGoingOutTrasferModeEvent ev && ev.CardTakerId.Equals(_takerId); ;
            }
            else
            {
                return @event is PlayersGoingOutTrasferModeEvent ev && ev.CardGiverId.Equals(_giverId);
            }
        }
    }
}