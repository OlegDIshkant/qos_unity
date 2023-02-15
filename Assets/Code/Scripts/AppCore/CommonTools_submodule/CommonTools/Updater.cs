using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace CommonTools
{
    /// <summary>
    /// Allows execute some action in a loop.
    /// </summary>
    public static class Updater
    {
        public delegate Task LoopDelegate(Action ToStopUpdate);


        /// <param name="action"> Call it to break the loop and exit the method. </param
        public static async Task LoopAsync(LoopDelegate action)
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            bool stopLoop = false;
            Action ToStopLoop = () => { stopLoop = true; };

            while (!stopLoop)
            {
                sw.Reset();
                await action(ToStopLoop);
                sw.Stop();
                if (sw.ElapsedMilliseconds < 1)
                {
                    await Task.Delay(1); // Prevent infinite synhronous loop
                }
            }
        }
    }
}
