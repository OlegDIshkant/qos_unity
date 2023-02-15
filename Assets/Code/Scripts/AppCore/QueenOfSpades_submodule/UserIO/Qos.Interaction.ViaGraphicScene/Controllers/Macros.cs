using CommonTools;
using Qos.Domain.Events;
using System;


namespace Qos.Interaction.ViaGraphicScene
{
    public static class Macros
    {

        public static bool IsEndOfContiniousEvent<T>(this IEvent @event, out T contEvent)
            where T : IContiniousEvent
        {
            if (
                @event is EventContinuation continuation &&
                continuation.NormalizedTime == NormValue.Max &&
                continuation.RootEvent is T continiousEvent
                )
            {
                contEvent = continiousEvent;
                return true;
            }

            contEvent = default;
            return false;
        }


        public static bool IsContiniousEvent<E>(this IEvent @event, out E contEvent, out NormValue normTime)
            where E : IContiniousEvent
        {
            return @event.IsContiniousEvent(e => true, out contEvent, out normTime);
        }


        public static bool IsContiniousEvent<E>(this IEvent @event, Func<E, bool> ExtraCondition, out E contEvent, out NormValue normTime)
            where E : IContiniousEvent
        {
            if (@event is E ev && ExtraCondition(ev))
            {
                contEvent = ev;
                normTime = NormValue.Min;
                return true;
            }
            else if (
                @event is EventContinuation continuation &&
                continuation.RootEvent is E rootEv &&
                ExtraCondition(rootEv))
            {
                contEvent = rootEv;
                normTime = continuation.NormalizedTime;
                return true;
            }

            contEvent = default;
            normTime = default;
            return false;
        }
    }   
}
