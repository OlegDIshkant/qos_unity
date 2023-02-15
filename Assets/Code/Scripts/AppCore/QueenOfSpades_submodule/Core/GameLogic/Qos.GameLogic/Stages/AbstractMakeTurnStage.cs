using CommonTools;
using Qos.GameLogic.GameWorld.Activities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Qos.GameLogic.GameWorld.Helper;


namespace Qos.GameLogic.GameWorld.Stages
{
    internal class AbstractMakeTurnStage : AbstractStage
    {
        private readonly IWhatActivitiesStrategy _whatActivitiesStrategy;
        private readonly AbstractTimeoutStrategy _timeoutStrategy;


        private ICollection<AbstractPlayerActivity> _playersActivities;
        private bool _allPlayerActivitiesFinished = false;


        public AbstractMakeTurnStage(
            IWhatActivitiesStrategy whatActivitiesStrategy,
            AbstractTimeoutStrategy timeoutStrategy)
        {
            _whatActivitiesStrategy = whatActivitiesStrategy;
            _timeoutStrategy = timeoutStrategy;
        }


        public override IEnumerator Complete()
        {
            var concurrentActions = new List<IEnumerator>()
            {
                RunningPlayerActivites(),
                TimeoutForStage()
            };

            yield return IteratateSimultaneously(concurrentActions);
        }


        private IEnumerator RunningPlayerActivites()
        {
            _playersActivities = NewPlayerActivities();
            yield return IteratateSimultaneously(Start(_playersActivities));
            _allPlayerActivitiesFinished = true;
        }


        private IEnumerable<IEnumerator> Start(ICollection<AbstractPlayerActivity> activities) =>
            activities.Select(
                activity => activity.StartExecute())
            .ToList();


        private ICollection<AbstractPlayerActivity> NewPlayerActivities() =>
            _whatActivitiesStrategy.DefinePlayerActivitiesForTurn();


        /// <summary>
        /// Время на "сброс" карт ограничен. 
        /// В добавок, стадия может кончится раньше, если все игроки завершат свои дела до таймаута.
        /// </summary>
        private IEnumerator TimeoutForStage()
        {
            var waitForTimeOut = RoutineHelper.RecursiveIterator(_timeoutStrategy.WaitingForTimeout());
            var waitForEarlyFinish = RoutineHelper.WaitingWhile(() => !_allPlayerActivitiesFinished);

            while (true)
            {
                yield return null;
                if (!waitForTimeOut.MoveNext())
                {
                    break;
                }
                if (!waitForEarlyFinish.MoveNext())
                {
                    _timeoutStrategy.WhenNoMoreNeedInTimeout();
                    break;
                }
            }

            if (!_allPlayerActivitiesFinished)
            {
                InterruptPlayerActivities();
            }
        }


        private void InterruptPlayerActivities()
        {
            foreach (var activity in _playersActivities)
            {
                if (activity.IsExecuting)
                {
                    activity.Cancel();
                }
            }
        }


        /// <summary>
        /// Определяет, чем заняты игроки во время хода.
        /// </summary>
        internal interface IWhatActivitiesStrategy
        {
            ICollection<AbstractPlayerActivity> DefinePlayerActivitiesForTurn();
        }


    }
}