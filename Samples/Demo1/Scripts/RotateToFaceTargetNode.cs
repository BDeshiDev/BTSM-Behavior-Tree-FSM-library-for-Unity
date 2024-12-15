using Bdeshi.BTSM.Samples.Demo1;
using Bdeshi.Helpers.Utility;
using BDeshi.BTSM;
using UnityEngine;

namespace Bdeshi.BTSM.Samples.Demo1
{
    /// <summary>
    /// Rotate to face target for x amount of time
    /// </summary>
    public class RotateToFaceTargetNode : BtNodeBase
    {
        public Transform target;
        public BasicMoveComponent mover;
        public FiniteTimer rotateTimer;

        public RotateToFaceTargetNode(Transform target, BasicMoveComponent mover, float rotationDuration)
        {
            this.target = target;
            this.mover = mover;
            rotateTimer = new FiniteTimer(rotationDuration);
        }

        public override void Enter()
        {
            rotateTimer.reset();
        }

        public override void Exit()
        {

        }

        public override BTStatus InternalTick()
        {
            mover.setLookDirection((target.position - mover.transform.position).normalized);
            if (rotateTimer.tryCompleteTimer(Time.deltaTime))
            {
                return BTStatus.Success;
            }
            return BTStatus.Running;
        }
    }
}