using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace BDeshi.BTSM
{
    public interface IState
    {
        void EnterState();
        void Tick();
        void ExitState();
        string Prefix { get; set; }
        string FullStateName { get; }
        string Name{ get; }
        [CanBeNull] IState Parent { get; set; }
    }

    public interface MultiState: IState
    {
        IEnumerable<IState> getChildStates();
        IState getActiveState { get; }
    }

    /// <summary>
    /// Implement this interface for monobehavior based states, if you want to avoid inheriting MonoBehaviourStateBase
    /// But still have editor state click -> object select and other functionality
    /// </summary>
    public interface IMonoBehaviorState : IState
    {
        GameObject gameObject { get; }
    }
}