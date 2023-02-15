using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace CommonTools
{
    public static class CollectionExtension
    {
        public static (int index, float min) FindIndexWithMin<T>(this IEnumerable<T> collection, Func<T, float> CalcValue)
        {
            float min = float.MaxValue;
            int resultIndex = 0;

            var index = 0;
            foreach (var item in collection)
            {
                var value = CalcValue(item);
                if (value <= min)
                {
                    min = value;
                    resultIndex = index;
                }
                index++;
            }

            return (resultIndex, min);
        }


        public static string MembersToString(this IEnumerable enumerable)
        {
            var sb = new StringBuilder(); sb.Append("[");
            foreach (var item in enumerable)
            {
                sb.Append($"{item}, ");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
