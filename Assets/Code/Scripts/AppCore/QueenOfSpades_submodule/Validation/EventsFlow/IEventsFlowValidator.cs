using Qos.Domain.Events;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Qos.Validation.EventsFlow
{
    /// <summary>
    /// ���������� ����� ������� �������.
    /// </summary>
    public interface IEventsFlowValidator 
    {
        /// <summary>
        /// ������ ����� ������� ������� ������ ���� �������� � ������� ��������� 
        /// � ������ ����� ��� �������� �� ������� ���������� �� ��������� "������".
        /// </summary>
        /// <returns>
        /// ���� ����������� ������ ������� ���� ��� �� ����������, ������ ������� �� ����������.
        /// </returns>
        IEnumerable<Problem> CheckNextEvent(IEvent @event);
    }


    /// <summary>
    /// �������� �����-���� ��������.
    /// </summary>
    public struct Problem
    {
        public string Message { get; private set; }

        public Problem(string msg)
        {
            Message = msg;
        }
    }
}