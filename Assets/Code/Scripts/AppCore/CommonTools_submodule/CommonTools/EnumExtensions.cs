using System;


namespace Assets.Code.Scripts.AppCore.CommonTools_submodule.CommonTools
{
    /// <summary>
    /// Доп. методы для работы с enum.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Случайный элемент в перечислении <typeparamref name="T"/>.
        /// </summary>
        public static T GetRandom<T>(Random rnd = null)
        {
            if (rnd == null)
            {
                rnd = new Random();
            }

            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(rnd.Next(0, values.Length));
        }
    }
}
