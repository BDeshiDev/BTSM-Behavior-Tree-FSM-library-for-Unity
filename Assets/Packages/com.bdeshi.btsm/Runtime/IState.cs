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
}