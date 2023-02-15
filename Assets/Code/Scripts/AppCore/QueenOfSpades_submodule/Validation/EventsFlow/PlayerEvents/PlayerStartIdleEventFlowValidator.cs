using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections.Generic;


namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// ���������, �����������, ��� ������� <see cref="PlayerStartIdleEvent"/> ����������, � ������ �������.
    /// </summary>
    internal class PlayerStartIdleEventFlowValidator : IEventsFlowValidator
    {
        private enum States { NONE, IDLING, DISCARD_MODE, TRANSFER_MODE }

        private readonly PlayerId _playerId;

        private States _currentState = States.NONE;


        public PlayerStartIdleEventFlowValidator(PlayerId player)
        {
            _playerId = player;
        }


        public IEnumerable<Problem> CheckNextEvent(IEvent @event)
        {
            if (@event is PlayerStartIdleEvent psiEvent)
                return Handle_PlayerStartIdle(psiEvent);
            else if (@event is PlayerGoingToDiscardModeEvent pgtdmEvent)
                return Handle_PlayerGoingToDiscardMode(pgtdmEvent);
            else if (@event is PlayersGoingToTrasferModeEvent pgttmEvent)
                return Handle_PlayerGoingToTramsferMode(pgttmEvent);

            return null;
        }


        private IEnumerable<Problem> Handle_PlayerStartIdle(PlayerStartIdleEvent @event)
        {
            if (!@event.PlayerId.Equals(_playerId)) return null;
            _currentState = States.IDLING;
            return null;
        }


        private IEnumerable<Problem> Handle_PlayerGoingToDiscardMode(PlayerGoingToDiscardModeEvent @event)
        {
            if (!@event.PlayerId.Equals(_playerId)) return null;

            if (_currentState != States.IDLING)
            {
                return new Problem("������� ������ �������� � ����� ������ ������ ����������� ����� ����, ��� ������������ ����� �������.").WrapInNewList();
            }
            _currentState = States.DISCARD_MODE;
            return null;
        }


        private IEnumerable<Problem> Handle_PlayerGoingToTramsferMode(PlayersGoingToTrasferModeEvent @event)
        {
            if (!@event.CardGiverId.Equals(_playerId) && !@event.CardTakerId.Equals(_playerId)) return null;

            if (_currentState != States.IDLING)
            {
                return new Problem("������� ������ �������� � ����� ������ ������ ����������� ����� ����, ��� ������������ ����� �������.").WrapInNewList();
            }
            _currentState = States.TRANSFER_MODE;
            return null;
        }



    }
}