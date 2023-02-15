using CommonTools;
using Qos.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Qos.GameLogic.GameWorld
{
    internal static class Helper
    {
        /// <summary>
        /// Сначала вернуть событие <paramref name="continiousEvent"/>, а затем в течении <paramref name="duration"/> миллисекунд 
        /// возвращать его продолжения в виде <seealso cref="EventContinuation"/>.
        /// </summary>
        public static IEnumerator RaiseContiniousEvent(IContiniousEvent continiousEvent, float duration, TimeContext timeContext)
        {
            yield return EachYieldInSeparateFrame(timeContext, RaisingContiniousEvent(continiousEvent, duration, timeContext));

        }

        private static IEnumerator RaisingContiniousEvent(IContiniousEvent continiousEvent, float duration, TimeContext timeContext)
        {
            yield return continiousEvent;
            yield return RaiseContinuations(continiousEvent, duration, timeContext);
        }


        private static IEnumerator RaiseContinuations(IContiniousEvent rootEvent, float duration, TimeContext timeContext)
        {
            var normTime = new NormValue();
            for (float time = timeContext.TimeSincePrevUpdate; time <= duration; time += timeContext.TimeSincePrevUpdate)
            {
                normTime = new NormValue(time / duration);
                yield return new EventContinuation(rootEvent, normTime);

            }

            // В конце возвращается событие с нормированным временем, равным 1.
            if (normTime != NormValue.Max)
            {
                yield return new EventContinuation(rootEvent, NormValue.Max);
            }
        }



        /// <summary>
        /// Рекурсивно(!) итерирует <seealso cref="IEnumerator"/>, возвращая игровые события.
        /// 
        /// <para>
        /// Создан для удобства! Благодаря выполнению <seealso cref="IEnumerator"/> внутри подобного метода, его код (и код его дочерних
        ///  <seealso cref="IEnumerator"/>) получится лаконичнее.
        /// </para>
        /// </summary>
        public static IEnumerator<IEnumerable<IEvent>> GameEventsIterator(IEnumerator rootRoutine)
        {
            var routinesStack = new Stack<IEnumerator>();
            routinesStack.Push(rootRoutine);

            while (routinesStack.Count > 0)
            {
            outerLoop:
                var routine = routinesStack.Peek();

                while (routine.MoveNext())
                {
                    // Мы не знаем наверняка, что может вернуть корутина, поэтому проверяяем тип
                    // возвращаемого объекта и при необходимости конвертируем в нужный.
                    if (routine.Current == null)
                    {
                        yield return null;
                    }
                    else if (routine.Current is IEvent @event)
                    {
                        yield return new List<IEvent>() { @event };
                    }
                    else if (routine.Current is IEnumerable someIEnumerable)
                    {
                        if (someIEnumerable is IEnumerable<IEvent> events)
                        {
                            yield return events;
                        }
                        else
                        {
                            // Иногда по ошибке можно написать у метода возвращаемое значение IEnumerable вместо IEnumerator
                            throw new System.Exception($"Don't know what to do with non-generic IEnumerable: '{someIEnumerable}'. Maybe it should be IEnumerator?");
                        }
                    }
                    else if (routine.Current is IEnumerator enumerator) //Должно быть после проверки на IEnumerable! 
                    {
                        routinesStack.Push(enumerator);
                        goto outerLoop;
                    }
                    else
                    {
                        throw new System.NotSupportedException($"Don't know what to do when routine returns: {routine.Current}");
                    }
                }

                routinesStack.Pop();
            }
        
        }


        /// <summary>
        /// Итерирует <paramref name="routine"/>, но не чаще чем
        /// раз в одно обновление <paramref name="timeContext"/>.
        /// </summary>
        public static IEnumerator EachYieldInSeparateFrame(TimeContext timeContext, IEnumerator routine)
        {
            while (true)
            {
                var waitingNextFrame = timeContext.StartWaitForNextUpdate();

                if (routine.MoveNext())
                {
                    yield return routine.Current;
                }
                else
                {
                    yield break;
                }

                yield return waitingNextFrame;
            }
        }


        /// <summary>
        /// Итерирует одновременно.
        /// </summary>
        public static IEnumerator IteratateSimultaneously(params IEnumerator[] routines)
        {
            return IteratateSimultaneously(routines.ToList());
        }


        /// <summary>
        /// Итерирует одновременно.
        /// </summary>
        public static IEnumerator IteratateSimultaneously(IEnumerable<IEnumerator> routines)
        {
            var recursiveRoutines = routines.AsEnumerable().Select(r => GameEventsIterator(r)).ToList();

            var events = new List<IEvent>();
            var allRoutinesFinished = false;

            while (!allRoutinesFinished)
            {
                allRoutinesFinished = true;
                events.Clear();
                foreach (var routine in recursiveRoutines)
                {
                    if (routine.MoveNext())
                    {
                        if (routine.Current != null)
                        {
                            events.AddRange(routine.Current);
                        }
                        allRoutinesFinished = false;
                    }
                }

                yield return events;
            }
        }



        /// <summary>
        /// Позволяет одновременно итерировать рутины, попутно добавляя новые.
        /// </summary>
        public class RoutinePool
        {
            private Queue<IEnumerator> _addQueue = new Queue<IEnumerator>();
            private List<IEnumerator<IEnumerable<IEvent>>> _runnigRoutines = new List<IEnumerator<IEnumerable<IEvent>>>();
            private List<IEnumerator<IEnumerable<IEvent>>> _finishedRoutines = new List<IEnumerator<IEnumerable<IEvent>>>();
            private IEnumerator<List<IEvent>> _infiniteLoop;


            public bool HasWorkToDo => 
                _runnigRoutines.Count > 0 || 
                _addQueue.Count > 0;


            public RoutinePool()
            {
                _infiniteLoop = IterateRoutines();
            }


            /// <summary>
            /// Добавить рутину для итерации.
            /// </summary>
            public void AddRoutine(IEnumerator routine)
            {
                _addQueue.Enqueue(routine);
            }


            /// <summary>
            /// Одновременно итерировать добавленные рутины.
            /// </summary>
            public List<IEvent> Iterate()
            {
                InjectNewRoutines();
                return _infiniteLoop.GetNext();
            }


            private void InjectNewRoutines()
            {
                while (_addQueue.Count > 0)
                {
                    _runnigRoutines.Add(GameEventsIterator(_addQueue.Dequeue()));
                }
            }


            private IEnumerator<List<IEvent>> IterateRoutines()
            {
                while (true)
                {
                    PrepareForIteration();
                    yield return Iteration();
                    RemoveFinishedRoutines();
                }
            }


            private void PrepareForIteration()
            {
                _finishedRoutines.Clear();
            }


            private List<IEvent> Iteration()
            {
                var returnedEvents = new List<IEvent>();
                foreach (var routine in _runnigRoutines)
                {
                    if (routine.MoveNext())
                    {
                        if (routine.Current != null)
                        {
                            returnedEvents.AddRange(routine.Current);
                        }
                    }
                    else
                    {
                        _finishedRoutines.Add(routine);
                    }
                }
                return returnedEvents;
            }


            private void RemoveFinishedRoutines()
            {
                foreach (var routineToDelete in _finishedRoutines)
                {
                    _runnigRoutines.Remove(routineToDelete);
                }
            }

        }

    }


}
