using UnityEngine;

namespace BDeshi.BTSM
{
    public class FSMRunner: MonoBehaviour
    {
        public IRunnableStateMachine fsm;
        /// <summary>
        /// Calls fsm.enter
        /// </summary>
        public  void Initialize(IRunnableStateMachine fsm, bool callEnter = true)
        {
            this.fsm = fsm;
            fsm.DebugContext = gameObject;
            fsm.enter(callEnter);
        }
        

        public bool shouldTickAutomatically = true;
        /// <summary>
        /// Manually tick FSM.
        /// </summary>
        public void manualTick()
        {
            fsm.Tick();
        }



        /// <summary>
        /// Just ticks FSM.
        /// </summary>
        protected virtual void Update()
        {
            if(shouldTickAutomatically)
                fsm.Tick();
        }
        
    }
}