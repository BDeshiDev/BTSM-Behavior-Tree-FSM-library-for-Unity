using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Maintains a FSM
    /// Calls tick,enter etc on it
    /// The Custom Editor Script looks for this.
    /// While you can update the fsm with a different class, it won't show up in the editor without this
    /// </summary>
    public class FSMRunner: MonoBehaviour
    {
        public StateMachine fsm;
        /// <summary>
        /// Calls fsm.enter
        /// </summary>
        public  void Initialize(StateMachine fsm, bool callEnter = true)
        {
            this.fsm = fsm;
            fsm.DebugContext = gameObject;
            fsm.enter(callEnter);
        }
        

        
        /// <summary>
        /// Just ticks FSM.
        /// </summary>
        protected virtual void Update()
        {
            fsm.Tick();
        }

    }
}