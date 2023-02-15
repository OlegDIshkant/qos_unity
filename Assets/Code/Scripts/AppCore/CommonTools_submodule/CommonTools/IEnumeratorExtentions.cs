using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CommonTools
{
    public static class IEnumeratorExtentions
    {
        /// <summary>
        /// Возвращает итератор для бесконечного цикличного итерирования элементов <paramref name="enumerable"/>.
        /// </summary>
        public static IEnumerator<T> IterateInCycle<T>(this IEnumerable<T> enumerable)
        {
            var iterator = enumerable.GetEnumerator();
            while (true)
            {
                if (!iterator.MoveNext())
                {
                    iterator = enumerable.GetEnumerator();
                    continue;
                }

                yield return iterator.Current;
            }
        }



        /// <summary>
        /// Может выжывать ошибки! 
        /// Итерирует вперед, предполагая, что следующий элемент есть.
        /// </summary>
        public static T GetNext<T>(this IEnumerator<T> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }



        public static List<T> WrapInNewList<T>(this T item)
        {
            return new List<T>() { item };
        }
    }
}
