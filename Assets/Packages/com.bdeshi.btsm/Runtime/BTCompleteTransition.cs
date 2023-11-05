using System;
using UnityEditorInternal;
using UnityEngine;

namespace BDeshi.BTSM
{
    public class BTCompleteTransition<TState> : Transition<TState>
    where TState:IState
    {
        private IBtNode node;
        public IState SuccessState => SuccessTypedState;
        public bool TakenLastTime { get; set; }
        public bool TransitionToSameState { get; set; }
        public Action OnTaken { get; }

        public TState SuccessTypedState { get; private set; }

        public BTCompleteTransition(IBtNode node, TState typedState)
        {
            this.node = node;
            this.SuccessTypedState = typedState;
            // Debug.Log("??" + (successState == null?"nnnnnulll":successState.EditorName));
        }

        public bool Evaluate()
        {
            if (node.LastStatus == BTStatus.Success)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"{node.EditorName}.complete->{SuccessState.FullStateName}";
        }
    }
}