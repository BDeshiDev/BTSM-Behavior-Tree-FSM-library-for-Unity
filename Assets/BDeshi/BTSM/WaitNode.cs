using BDeshi.Utility;
using UnityEngine;

namespace BDeshi.BTSM
{
    public class WaitNode: BtNodeBase
    {
        protected FiniteTimer timer;

        public WaitNode(float waitTime = 1.9f)
        {
            timer = new FiniteTimer(waitTime);
        }

        public override void Enter()
        {
            timer.reset();
        }

        public override BTStatus InternalTick()
        {
            if (timer.tryCompleteTimer(Time.deltaTime))
            {
                return BTStatus.Success;
            }

            return BTStatus.Running;
        }

        public override void Exit()
        {
           
        }
    }
}