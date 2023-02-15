using System;
using System.Collections;


namespace Qos.GameLogic.GameWorld.Activities
{
    /// <summary>
    /// Некое дело, которым занят игрок.
    /// </summary>
    internal abstract class AbstractPlayerActivity
    {
        public bool IsExecuting { get; private set; } = false;
        public bool IsCancelled { get; private set; }


        public IEnumerator StartExecute()
        {
            if (IsExecuting || !AllowToStartExecution())
            {
                throw new InvalidOperationException("Already executing");
            }
            IsExecuting = true;
            IsCancelled = false;

            yield return Executing();

            IsExecuting = false;
        }


        protected virtual bool AllowToStartExecution() => true;


        protected abstract IEnumerator Executing();


        /// <summary>
        /// Просит процесс, начатый через <see cref="StartExecute"/>, остановиться.
        /// </summary>
        /// <remarks>
        /// После вызова метода нужно продолжать итерировать рутину, полученную через <see cref="StartExecute"/>,
        /// до её полной остановки!
        /// </remarks>
        public void Cancel()
        {
            if (!AllowCancelling)
            {
                throw new NotSupportedException();
            }
            if (!IsExecuting)
            {
                throw new InvalidOperationException("Proccess has not been started yet!");
            }
            IsCancelled = true;
        }


        protected virtual bool AllowCancelling => true;
    }
}