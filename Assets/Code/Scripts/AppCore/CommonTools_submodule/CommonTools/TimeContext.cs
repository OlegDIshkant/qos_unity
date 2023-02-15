using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace CommonTools
{
    /// <summary>
    /// Помогает с различными таймингами в бесконечном Update-цикле.
    /// </summary>
    public class TimeContext
    {
        private Stopwatch _sw = new Stopwatch();
        private Action _OnUpdate;
        private LinkedList<Timer> _timers = new LinkedList<Timer>();
        private ulong _frameCounter;

        public int TimeSincePrevUpdate { get; private set; }


        public TimeContext(out Action UpdateAction)
        {
            UpdateAction = Update;
        }


        /// <summary>
        /// Нужно вызывать в начале каждой новой итерации цикла.
        /// </summary>
        private void Update()
        {
            UpdateFrameCounter();
            UpdateTimings();
            UpdateTimers();
            NotifyAboutNewUpdate();
        }


        private void UpdateFrameCounter()
        {
            _frameCounter = (_frameCounter == uint.MaxValue) ? uint.MinValue : _frameCounter + 1;
        }


        /// <summary>
        /// Помогает корутинам дожидаться начала следующего обновления.
        /// </summary>
        public IEnumerator StartWaitForNextUpdate()
        {
            var updateReached = false;
            _OnUpdate += () => { updateReached = true; };
            return RoutineHelper.WaitingWhile(waitWhile: () => !updateReached);
        }


        public OncePerFrame NewOncePerFrameExecutor() => new OncePerFrame(this);


        public IEnumerator WaitTime(int millliseconds)
        {
            var timerKeeper = _timers.AddLast(new Timer(millliseconds));
            while (!timerKeeper.Value.HasExpired) // он должен уменьшаться вне данного метода
            {
                yield return null;
            }
            _timers.Remove(timerKeeper);
        }


        private void UpdateTimings()
        {
            _sw.Stop();
            TimeSincePrevUpdate = checked((int)_sw.ElapsedMilliseconds); // Предполагаем, что оверфлоата не случится
            _sw.Restart();
        }


        private void UpdateTimers()
        {
            foreach (var timer in _timers)
            {
                timer.TimePassed(TimeSincePrevUpdate);
            }
        }


        private void NotifyAboutNewUpdate()
        {
            _OnUpdate?.Invoke();
            _OnUpdate = null;   // Обязательно сбрасываем! Иначе будем оповещать и о следующих обновлениях тоже.
        }


        private class Timer
        {
            private int _millisecLeft;

            public bool HasExpired => _millisecLeft <= 0;

            public Timer(int milliseconds)
            {
                _millisecLeft = Math.Max(0, milliseconds);
            }

            public void TimePassed(int milliseconds)
            {
                _millisecLeft = Math.Max(0, _millisecLeft - milliseconds);
            }
        }



        public class OncePerFrame
        {
            private TimeContext _parent;
            private ulong _lastFrame;

            public OncePerFrame(TimeContext parent)
            {
                _parent = parent;
                _lastFrame = NotThisFrame(_parent._frameCounter);
            }


            private ulong NotThisFrame(ulong currentFrame)
            {
                return (currentFrame > ulong.MinValue) ? ulong.MinValue : ulong.MaxValue;
            }


            public void Execute(Action OncePerFrameAction, Action IfSameFrameAction = null)
            {
                if (_lastFrame != _parent._frameCounter)
                {
                    OncePerFrameAction?.Invoke();
                    _lastFrame = _parent._frameCounter;
                }
                else
                {
                    IfSameFrameAction?.Invoke();
                }
            }
        }
    }
}
