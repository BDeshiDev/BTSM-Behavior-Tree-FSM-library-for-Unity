using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Interface for a state
    /// </summary>
    public interface State
    {
        /// <summary>
        ///  Called when state is entered
        /// </summary>
        void EnterState();
        /// <summary>
        /// Called to update the state from fsm.tick()
        /// </summary>
        void Tick();
        /// <summary>
        ///  Called when state is exited
        /// </summary>
        void ExitState();
        string Prefix { get; set; }
        /// <summary>
        /// The name that shows up in the editor
        /// </summary>
        string FullStateName { get; }
        string Name{ get; }
        [CanBeNull] State Parent { get; set; }
    }
    /// <summary>
    /// State interface for states that have children
    /// You can manage child states without this
    /// but implementing this is needed for them to be detected by the editor view
    /// </summary>
    public interface ContainerState: State
    {
        /// <summary>
        /// Returns children states under this state
        /// </summary>
        /// <returns></returns>
        IEnumerable<State> getChildStates();
    }
    
    /// <summary>
    /// Implement this interface for monobehavior based states, if you want to avoid inheriting MonoBehaviourStateBase
    /// But still have editor state click -> object select and other functionality
    /// </summary>
    public interface MonoBehaviorState : State
    {
        GameObject gameObject { get; }
    }
}