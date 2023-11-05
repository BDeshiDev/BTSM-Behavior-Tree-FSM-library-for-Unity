using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BDeshi.BTSM
{
    /// <summary>
    /// General interface for Transitions
    /// </summary>
    public interface Transition
    {

        /// Returns null if evaluation fails
        /// </summary>
        /// <returns></returns>
        bool Evaluate();
        
        State SuccessState { get; }
        bool TakenLastTime { get; set; }
        bool TransitionToSameState { get; set; }
        [CanBeNull]public Action OnTaken { get; }
    }

    public class StateCondition
    {
        public bool defaultValue;
        private StateMachine fsm;
        private readonly Dictionary<State, Func<bool>> conditionFuncs;

        //apparently normal func => delegate invokes GC
        static readonly Func<bool> alwaysTrue = () => true;
        static readonly Func<bool> alwaysFalse = () => false;


        public StateCondition addStateConditionFunc(State state, Func<bool> conditionFunc)
        {
            conditionFuncs.Add(state,conditionFunc);
            return this;
        }
        
        
        public StateCondition addStateConditionFalse(State state)
        {
            conditionFuncs.Add(state,alwaysFalse);
            return this;
        }
        
        public StateCondition addStateConditionTrue(State state)
        {
            conditionFuncs.Add(state,alwaysTrue);
            return this;
        }

        public StateCondition(StateMachine fsm, bool defaultValue)
        {
            this.fsm = fsm;
            this.defaultValue = defaultValue;
            conditionFuncs = new Dictionary<State, Func<bool>>();
        }

        public bool evaluate()
        {
            if (conditionFuncs.TryGetValue(fsm.curState, out var func))
                return func();
            return defaultValue;
        }
    }
}