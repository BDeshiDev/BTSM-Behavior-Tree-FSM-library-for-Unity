using BDeshi.BTSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bdeshi.BTSM.Samples.Demo1
{
    public class PatrolBTNode : BtNodeBase
    {
        public List<Transform> path;
        BasicMoveComponent mover;
        int pathIndex = 0;
        public float stopDist = 1.69f;
        public float moveSpeed;

        public PatrolBTNode(List<Transform> path, BasicMoveComponent mover, float moveSpeed = 4)
        {
            this.path = path;
            this.mover = mover;
            this.moveSpeed = moveSpeed; 
        }

        public override void Enter()
        {
            pathIndex = 0;
            mover.speed = moveSpeed;
        }

        public override void Exit()
        {
            
        }

        public override BTStatus InternalTick()
        {
            if (pathIndex >= path.Count)
                return BTStatus.Success;
            Vector3 dirToTarget = path[pathIndex].position - mover.transform.position;
            dirToTarget.y = 0;
            var distToTarget = dirToTarget.magnitude;
            if(distToTarget <= stopDist)
            {
                pathIndex++;
            }
            else
            {
                mover.MoveInputNextFrame = dirToTarget.normalized;
                mover.setLookDirection(dirToTarget);
            }
            return BTStatus.Running;
        }
    }
}
