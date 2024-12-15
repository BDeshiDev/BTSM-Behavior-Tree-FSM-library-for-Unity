using Bdeshi.BTSM.Samples.Demo1;
using Bdeshi.Helpers.Utility;
using BDeshi.BTSM;
using UnityEngine;

namespace Bdeshi.BTSM.Samples.Demo1
{
    public class ChargeTowardsTargetNode : BtNodeBase
    {
        public Transform target;
        public BasicMoveComponent mover;
        public FiniteTimer chargeDurationTimer;
        public float chargeBaseSpeed;
        Vector3 chargeDir;
        private float chargeDeacceleration = 2;

        public ChargeTowardsTargetNode(Transform target, BasicMoveComponent mover, float chargeDuration, float chargeBaseSpeed = 8)
        {
            this.target = target;
            this.mover = mover;
            chargeDurationTimer = new FiniteTimer(chargeDuration);
            this.chargeBaseSpeed = chargeBaseSpeed;
        }

        public override void Enter()
        {
            chargeDurationTimer.reset();
            chargeDir = mover.transform.forward;

            mover.speed = chargeBaseSpeed;
        }

        public override void Exit()
        {

        }

        public override BTStatus InternalTick()
        {
            mover.MoveInputNextFrame = chargeDir;
            mover.speed = Mathf.MoveTowards(chargeBaseSpeed, 0, chargeDeacceleration * Time.deltaTime);
            if (chargeDurationTimer.tryCompleteTimer(Time.deltaTime))
            {
                return BTStatus.Success;
            }
            return BTStatus.Running;
        }
    }
}