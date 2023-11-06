using System.Collections;
using UnityEngine;
using BDeshi.BTSM;
using Bdeshi.BTSM.Samples.Demo1;

public class TargetChaseState : StateBase
{
    BasicMoveComponent mover;
    public Transform target;
    public float stopDist = 2f;
    private BasicMoveComponent _moveComponent;
    private Transform _player;

    public TargetChaseState(BasicMoveComponent mover, Transform target, float stopDist)
    {
        this.mover = mover;
        this.target = target;
        this.stopDist = stopDist;
    }

    public TargetChaseState(BasicMoveComponent moveComponent, Transform player)
    {
        _moveComponent = moveComponent;
        _player = player;
    }

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void Tick()
    {
        var dirToTarget = target.position - mover.transform.position;
        Vector3 dirToTargetNormalized = dirToTarget.normalized;

        var distToTarget = dirToTarget.magnitude;
        if(distToTarget > stopDist)
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
