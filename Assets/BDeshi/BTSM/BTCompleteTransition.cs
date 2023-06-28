using System;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Transition that will be taken when a BTNode succeeeds
    /// </summary>
    public class BTCompleteTransition : Transition
    {
        /// <summary>
        /// The BT node that will be tracked
        /// </summary>
        private IBtNode node;
        public State SuccessState => successState;
        public bool TakenLastTime { get; set; }
        public bool TransitionToSameState { get; set; }
        public Action OnTaken { get; }
        /// <summary>
        /// The state that the transition will pick if the BTNode succeeds
        /// </summary>
        public State successState;

        public BTCompleteTransition(IBtNode node, State state)
        {
            this.node = node;
            this.successState = state;
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