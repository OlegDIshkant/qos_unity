using System;
using System.Collections;
using System.Collections.Generic;


namespace CommonTools
{
    public static class RoutineHelper
    {
        /// <summary>
        /// До конца проитерировать корутину <paramref name="routine"/>,
        /// а затем выполнить <paramref name="OnFinished"/>.
        /// </summary>
        public static IEnumerator ExecuteWithAfteraction(IEnumerator routine, Action OnFinished)
        {
            while (routine.MoveNext())
            {
                yield return routine.Current;
            }
            OnFinished();
        }


        /// <summary>
        /// Возвращает <see cref="null"/> пока <paramref name="waitWhile"/> не вернет <see cref="false"/>,
        /// а затем завершается.
        /// </summary>
        public static IEnumerator WaitingWhile(Func<bool> waitWhile)
        {
            while (waitWhile())
            {
                yield return null;
            }
        }


        /// <summary>
        /// Рекурсивно(!) итерирует <seealso cref="IEnumerator"/>.
        /// 
        /// <para>
        /// Создан для удобства! Благодаря выполнению <seealso cref="IEnumerator"/> внутри подобного метода, его код (и код его дочерних
        ///  <seealso cref="IEnumerator"/>) получится лаконичнее.
        /// </para>
        /// </summary>
        public static IEnumerator<IEnumerable> RecursiveIterator(IEnumerator rootRoutine)
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
                    else if (routine.Current is IEnumerable someIEnumerable)
                    {
                        // Иногда по ошибке можно написать у метода возвращаемое значение IEnumerable вместо IEnumerator
                        throw new System.Exception($"Don't know what to do with non-generic IEnumerable: '{someIEnumerable}'. Maybe it should be IEnumerator?");
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
    }
}
