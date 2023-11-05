using System;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Basic transition class
    /// </summary>
    public class SimpleTransition: Transition
    {
        private State s;
        private Func<bool> evaluateFunc;

        /// <summary>
        /// Create a transition to a state
        /// </summary>
        /// <param name="s"></param>
        /// <param name="evaluateFunc">If NULL Transition will ALWAYS BE TRUE</param>
        /// <param name="onTaken">Executed if this is taken</param>
        public SimpleTransition(State s, Func<bool> evaluateFunc = null, Action onTaken= null)
        {
            this.s = s;
            this.evaluateFunc = evaluateFunc;
            OnTaken = onTaken;
        }

        public State SuccessState => s;
        public bool TakenLastTime { get; set; }
        public bool TransitionToSameState { get; set; } = false;
        public Action OnTaken { get; }

        /// <summary>
        /// If Func return true, else return false
        /// </summary>
        /// <returns></returns>
        bool Transition.Evaluate()
        {
            if (evaluateFunc == null || evaluateFunc.Invoke())
                return true;
            return false;
        }
    }
}