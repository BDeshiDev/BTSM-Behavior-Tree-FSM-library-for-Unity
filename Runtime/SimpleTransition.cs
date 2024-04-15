using System;

namespace BDeshi.BTSM
{
    public class SimpleTransition<TState>: Transition<TState>
    where TState: IState
    {
        private Func<bool> evaluateFunc;

        /// <summary>
        /// Create a transition to a state
        /// </summary>
        /// <param name="s"></param>
        /// <param name="evaluateFunc">If NULL Transition will ALWAYS BE TRUE</param>
        /// <param name="onTaken">Executed if this is taken</param>
        public SimpleTransition(TState s, Func<bool> evaluateFunc = null, Action onTaken= null)
        {
            this.SuccessTypedState = s;
            this.evaluateFunc = evaluateFunc;
            OnTaken = onTaken;
        }

        public IState SuccessState => SuccessTypedState;
        public bool TakenLastTime { get; set; }
        public bool TransitionToSameState { get; set; } = false;
        public TState SuccessTypedState { get; private set; }
        public Action OnTaken { get; }

        /// <summary>
        /// If Func return true, else return false
        /// </summary>
        /// <returns></returns>
        public bool Evaluate()
        {
            if (evaluateFunc == null || evaluateFunc.Invoke())
                return true;
            return false;
        }
    }
}