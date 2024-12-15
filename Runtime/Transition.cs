using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BDeshi.BTSM
{
    public interface TransitionBase
    {
        /// <summary>
        /// Returns true if evaluation succeeds
        /// </summary>
        /// <returns></returns>
        bool Evaluate();
        IState SuccessState { get; }
        bool TakenLastTime { get; set; }
        bool TransitionToSameState { get; set; }
    }
    /// <summary>
    /// General interface for Transitions
    /// </summary>
    public interface Transition<TState>: TransitionBase
        where TState : IState
    {
        TState SuccessTypedState { get; }
        [CanBeNull] public Action OnTaken { get; }
    }

}