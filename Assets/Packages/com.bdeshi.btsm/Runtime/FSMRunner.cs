using System;
using UnityEngine;

namespace BDeshi.BTSM
{
    public class FSMRunner: MonoBehaviour
    {
        public IRunnableStateMachine fsm;
        public bool ShouldLog = false;
        public bool shouldTickAutomatically = true;
        /// <summary>
        /// Calls fsm.enter
        /// </summary>
        public  void Initialize(IRunnableStateMachine fsm, bool callEnter = true)
        {
            this.fsm = fsm;
            fsm.DebugContext = gameObject;
            SyncDebugOnInFSM();
            fsm.Enter(callEnter);
        }

        private void OnValidate()
        {
            if(fsm != null)
                SyncDebugOnInFSM();
        }

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

        public void SyncDebugOnInFSM()
        {
            fsm.ShouldLog = ShouldLog;
        }
        
    }
}