using CommonTools.Exceptions;
using System;


namespace CommonTools.StatesManaging
{

    public class StateChecker<T>
        where T : Enum
    {
        public Enum CurrentState { get; private set; }
        private readonly Enum _disposedState;


        public StateChecker(Enum initialState)
        {
            SetInitialState(initialState);
        }


        public StateChecker(Enum initialState, Enum disposedState)
        {
            SetInitialState(initialState);

            if (!Enum.IsDefined(typeof(T), disposedState)) throw new InvalidStateException();
            _disposedState = disposedState;
        }


        private void SetInitialState(Enum initialState)
        {
            if (!Enum.IsDefined(typeof(T), initialState)) throw new InvalidStateException();
            CurrentState = initialState;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowedStates"></param>
        /// <returns></returns>
        public StateChecker<T> GoFurtherIf(params Enum[] allowedStates)
        {
            if (CurrentStateIsNot(allowedStates))
                throw new Exception($"Current state '{CurrentState}' is supposed to be different now.");

            return this;
        }


        private bool CurrentStateIsNot(params Enum[] states) =>
            null == Array.Find(states, (state) => CurrentState.Equals(state));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeStateTo(Enum newState)
        {
            if (!Enum.IsDefined(typeof(T), newState)) throw new InvalidStateException();
            CurrentState = newState ?? throw new DoesntExistEcxeption();
        }

    }
}
