using Bdeshi.Helpers.Utility;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Run child for x secs
    /// </summary>
    public class TimeoutNode: BTSingleDecorator
    {
        public FiniteTimer timer;
      
        public override void Enter()
        {
            timer.reset();
        }

        public override BTStatus InternalTick()
        {
            if (timer.isComplete)
            {
                return BTStatus.Success;
            }
            else
            {
                timer.safeUpdateTimer(Time.deltaTime);
                return child.Tick();
            }
        }

        public override void Exit()
        {
            
        }

        public TimeoutNode(BtNodeBase child, float timerDuration) : base(child)
        {
            timer = new FiniteTimer(timerDuration);
        }
    }
}