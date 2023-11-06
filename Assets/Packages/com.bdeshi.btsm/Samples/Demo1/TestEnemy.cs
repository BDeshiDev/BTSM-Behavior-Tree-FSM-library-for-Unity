using Bdeshi.BTSM.Samples.Demo1;
using BDeshi.BTSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bdeshi.BTSM.Samples.Demo
{
    [RequireComponent(typeof(BasicMoveComponent))]
    [RequireComponent (typeof(FSMRunner))]
    public class TestEnemy : MonoBehaviour
    {
        BasicMoveComponent moveComponent;
        Transform player;
        FSMRunner runner;

        List<Transform> patrolPath;
        // Use this for initialization
        void Awake()
        {
            moveComponent = GetComponent<BasicMoveComponent>();
            player = GameObject.FindWithTag("Player").transform;

            //ensure that you have a FSMRunner component
            runner = GetComponent<FSMRunner>();
            //make a state, BT not necessary
            var patrolState = new BTWrapperState(
                    new Repeat(
                            new PatrolBTNode(patrolPath, moveComponent)
                        )
                );
            var chaseState = new TargetChaseState(moveComponent, player);
            var attackState = new 
            //create a new statemachine, set initial state
            var fsm = new StateMachine<IState>(patrolState);
            //initialize the runner
            runner.Initialize(fsm);
        }


    }
}
