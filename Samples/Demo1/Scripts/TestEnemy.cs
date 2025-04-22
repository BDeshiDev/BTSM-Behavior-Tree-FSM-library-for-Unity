using Bdeshi.BTSM.Samples.Demo1;
using Bdeshi.Helpers.Utility;
using BDeshi.BTSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bdeshi.BTSM.Samples.Demo1
{
    [RequireComponent(typeof(BasicMoveComponent))]
    [RequireComponent (typeof(FSMRunner))]
    public class TestEnemy : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] List<Transform> patrolPath;
        BasicMoveComponent moveComponent;
        FSMRunner runner;
        public float aggroStartDistance = 4f;
        public float aggroEndDistance = 7f;
        public FiniteTimer attackCoolDownTimer = new FiniteTimer(6f);
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
            var attackState = new BTWrapperState(
                    new SequenceNode()
                        .appendChild(new RotateToFaceTargetNode(player, moveComponent, 1.2f))
                        .appendChild(new ChargeTowardsTargetNode(player, moveComponent, 1.2f))
                        .appendChild(new WaitNode(1.2f)
                    )
            );
            //create a new statemachine, set initial state
            var fsm = new StateMachine<IState>(patrolState);

            // patrolstate => chasestate if player is close
            fsm.addTransition(patrolState, chaseState,
                () => (player.position - transform.position).magnitude <= aggroStartDistance);
            // patrolstate => chasestate if player is far
            fsm.addTransition(chaseState, patrolState,
                () => (player.position - transform.position).magnitude >= aggroEndDistance);
            // chasestate => attackState if charge cooldown is complete
            fsm.addTransition(chaseState, attackState,
                () => attackCoolDownTimer.isComplete,
                () => attackCoolDownTimer.reset());// when this transition is taken, reset the cooldown
            // attackState.LastStatus == BTStatus.Success when the whole BT is executed
            fsm.addTransition(attackState, chaseState,
                () => attackState.LastStatus == BTStatus.Success);
     
            //initialize the runner
            runner.Initialize(fsm);
        }

        private void Update()
        {
            attackCoolDownTimer.safeUpdateTimer(Time.deltaTime);
        }


    }
}
