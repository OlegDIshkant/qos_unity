using CommonTools;
using System;
using System.Collections;


namespace Qos.GameLogic.GameWorld.Stages
{
    /// <summary>
    /// Таймаут для хода.
    /// </summary>
    internal abstract class AbstractTimeoutStrategy
    {
        protected bool NoMoreNeedInTimeout { get; private set; } = false;
        public bool Started { get; private set; } = false;
        public bool Finished { get; private set; } = false;


        public IEnumerator WaitingForTimeout()
        {
            if (Started)
            {
                throw new InvalidOperationException();
            }
            Started = true;

            yield return Running();
            Finished = true;
        }


        protected abstract IEnumerator Running();


        public void WhenNoMoreNeedInTimeout()
        {
            if (NoMoreNeedInTimeout || !Started || Finished)
            {
                throw new InvalidOperationException();
            }
            NoMoreNeedInTimeout = true;

            OnNoMoreNeedInTimeout();
        }


        protected abstract void OnNoMoreNeedInTimeout();
    }



    /// <summary>
    /// Обычный таймаут.
    /// </summary>
    internal class StandartTimeoutStrategy : AbstractTimeoutStrategy
    {
        private readonly int TIMEOUT_MILLISEC;// = 6 * 10 * 1000;

        private TimeContext _timeContext;


        public StandartTimeoutStrategy(TimeContext context, int timeoutMillisec)
        {
            _timeContext = context;
            TIMEOUT_MILLISEC = timeoutMillisec;
        }


        protected override IEnumerator Running()
        {
            var waiting = RoutineHelper.RecursiveIterator(_timeContext.WaitTime(TIMEOUT_MILLISEC));
            while (waiting.MoveNext() && !NoMoreNeedInTimeout)
            {
                yield return null;
            }
        }


        protected override void OnNoMoreNeedInTimeout()
        {
        }
    }



}