using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections.Generic;


namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// ��������� ������� ������ ������ �� �����.
    /// </summary>
    public class PlayersLeaveMatchEventsValidator : IEventsFlowValidator
    {
        private HashSet<PlayerId> _playersLeftMatch = new HashSet<PlayerId>();


        public IEnumerable<Problem> CheckNextEvent(IEvent @event)
        {
            if (@event is PlayerNotLostMatchEvent pnlmEvent)
                return Handle_PlayersNotLostMatch(pnlmEvent);

            return null;
        }


        private IEnumerable<Problem> Handle_PlayersNotLostMatch(PlayerNotLostMatchEvent ev)
        {
            var player = ev.PlayerId;

            if (PlayerAlreadyLeftMatch(player))
            {
                return new Problem($"����� '{player}' ��� ������� ���� �����.").WrapInNewList();
            }
            MarkPlayerAsOneLeftMatch(player);

            return null;
        }


        private bool PlayerAlreadyLeftMatch(PlayerId player) => _playersLeftMatch.Contains(player);
        private void MarkPlayerAsOneLeftMatch(PlayerId player) => _playersLeftMatch.Add(player);
    }
}