using UnityEngine;

namespace BDeshi.BTSM
{
    public class BTWrapperState: StateBase
    {
        public IBtNode BTRoot { get; private set; }
        private BTStatus lastStatus;
        public BTStatus LastStatus => lastStatus;

        /// <summary>
        /// Creates a transition to a state when the root BT node is complete.
        /// DOES NOT automagically go to statemachine.
        /// Do that yourself.
        /// </summary>
        /// <returns>Newly Created transition.</returns>
        public BTCompleteTransition createRootSuccessTransition(State to)
        {
            return new BTCompleteTransition(this.BTRoot, to);
        }

        public BTWrapperState(BtNodeBase btRoot)
        {
            this.BTRoot = btRoot;
        }

        public override void EnterState()
        {
            BTRoot.Enter();
        }

        public override void Tick()
        {
            lastStatus = BTRoot.Tick();
        }

        public override void ExitState()
        {
            BTRoot.Exit();
        }
    }
}