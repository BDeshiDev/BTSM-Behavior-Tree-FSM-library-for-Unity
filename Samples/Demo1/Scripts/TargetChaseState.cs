using System.Collections;
using UnityEngine;
using BDeshi.BTSM;
using Bdeshi.BTSM.Samples.Demo1;

namespace Bdeshi.BTSM.Samples.Demo1
{
    public class TargetChaseState : StateBase
    {
        BasicMoveComponent mover;
        public Transform target;
        public float stopDist = 2f;
        private BasicMoveComponent _moveComponent;
        private Transform _player;
        public float moveSpeed;

        public TargetChaseState(BasicMoveComponent mover, Transform target, float stopDist = 2f, float moveSpeed = 4f)
        {
            this.mover = mover;
            this.target = target;
            this.stopDist = stopDist;
            this.moveSpeed = moveSpeed;
        }


        public override void EnterState()
        {
            mover.speed = moveSpeed;
        }

        public override void ExitState()
        {

        }

        public override void Tick()
        {
            var dirToTarget = target.position - mover.transform.position;
            Vector3 dirToTargetNormalized = dirToTarget.normalized;

            var distToTarget = dirToTarget.magnitude;
            if (distToTarget > stopDist)
            {
                mover.MoveInputNextFrame = dirToTargetNormalized;
            }
            else
            {
                mover.MoveInputNextFrame = Vector3.zero;
            }

            mover.setLookDirection(dirToTargetNormalized);
        }
    }
}