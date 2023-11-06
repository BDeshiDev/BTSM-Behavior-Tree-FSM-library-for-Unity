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
        public float stopDist = 1.25f;

        public PatrolBTNode(List<Transform> path, BasicMoveComponent mover)
        {
            this.path = path;
            this.mover = mover;
        }

        public override void Enter()
        {
            pathIndex = 0;
        }

        public override void Exit()
        {
            
        }

        public override BTStatus InternalTick()
        {
            if (pathIndex > path.Count)
                return BTStatus.Success;
            Vector3 dirToTarget = path[pathIndex].position - mover.transform.position;
            var distToTarget = dirToTarget.magnitude;
            if(distToTarget <= stopDist)
            {
                pathIndex++;
            }
            else
            {
                mover.MoveInputNextFrame = dirToTarget.normalized;
            }
            return BTStatus.Running;
        }
    }
}