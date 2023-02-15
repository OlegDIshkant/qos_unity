using System;

namespace Qos.AI
{
    internal static class Helper
    {

        private static Random _rnd = new Random();

        public static int RandomDelay()
        {
            return _rnd.Next(100, 1000);
        }
    }
}
