# BTSM-Behavior-Tree-FSM-library-for-Unity
![GitHub Actions](https://github.com/BDeshiDev/BTSM-Behavior-Tree-FSM-library-for-Unity/actions/workflows/UPMBranchUpdate.yml/badge.svg)

BTSM is a Behavior Tree + Finite State Machine library for Unity.
- States, BT and Transitions are modular and are written entirely in code
- States can contain behavior trees.
- Supports Hierarchical Finite State Machines. States can contain child states.
- Can transition between states depending on func/lambda based conditions.
- Sequence, Parallel etc. common decorators are provided.
- Runtime debugging via custom editor window.
- Since it's fully code based, you can use your IDE to debug, refactor and reuse BT/States easily
<img src="https://github.com/BDeshiDev/BTSM-Behavior-Tree-FSM-library-for-Unity/assets/17526821/f0e6b3d5-9bee-4b6d-9678-f391be67cb23" height="360" />
<img src="https://github.com/BDeshiDev/BTSM-Behavior-Tree-FSM-library-for-Unity/assets/17526821/30f21259-0fa5-43bf-9563-00bf68bf0f00" height="360" />

# Manual
[Manual available here](https://bdeshidev.github.io/BTSM-Behavior-Tree-FSM-library-for-Unity/manual/) 
# Usage

## Basic example
### Creating the states, FSM
You will need to have a FSMRunner component on the Gameobject. Then:
```csharp
// ensure that you have a reference to a FSMRunner component
runner = GetComponent<FSMRunner>();
// make a state, BT not necessary
// this is an example state that just patrols repeatedly.
var patrolState = new BTWrapperState(
        new Repeat(
                new PatrolBTNode(patrolPath, moveComponent)
            )
    );
// create a new statemachine, set initial state
// Generic parameter can be any type implementing IState
var fsm = new StateMachine<IState>(patrolState);
// initialize the runner
runner.Initialize(fsm);
```
In the Editor:
1. Open the FSMEditorwindow.
1. Select the Gameobject with the FSMRunner.
1. The FSMEditorwindow window will work during playmode.
### Adding transitions
Transitions are added on the Statemachine object by evaluating a condition:
```csharp
Istate patrolState;
Istate chaseState;
//...
fsm.addTransition(patrolState, chaseState,
    () => (player.position - transform.position).magnitude <= aggroStartDistance);
```
Here, if distance to the player is less then aggroStartDistance, the fsm will go from patrolState to chaseState. This transition will only be evaluated when the current state is patrolState.
You can add any amount of transitions.
## A full example:
```csharp
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
            .appendChild(new ChargeTowardsTargetNode(player, moveComponent, 1.69f))
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
runner.Initialize(fsm);
```
### FAQ
## What types can states be?
Any type inheriting from `IState`.

 It can be a plain C# class or a monobehavior. `Monobehavior` states should implement `IMonoBehaviorState` instead of `IState`. Doing so will allow you to go to the gameobject when you click on the State in the FSMEditor window.

Serializable c# classes will also work but it's not recommended due to reference issues.
## What types can BT nodes be?
Any type inheriting from `IBtNode`. 

It can be a plain C# class or a monobehavior. `Monobehavior` states should implement `BtNodeMonoBase` instead of `IBtNode`. 

Serializable c# classes will also work but it's not recommended due to reference issues.

## Do I have to use a statemachine to use BTs?
No. You can create a BT independently and call `Enter()`, `Exit()`, `Tick()` etc. manually. Ex:

```csharp
IBTNode root;
void Awake(){
    root = new Repeat(
        new PatrolBTNode(patrolPath, moveComponent)
    );
}

void OnEnable(){
    root.Enter();
}

void Update(){
    root.Tick();
}

void OnDisable(){
    root.Exit();
}

```

## Can I make a state that doesn't use BT?
Yes. The statemechine can use anything implementing 'IState'. It has no dependency on BTs. You have the option of using BTs by using a BTWrapperState. 


But you can just implment IState and write whatever state logic you want. The `TargetChaseState` class in the samples is an example of a State class that doesn't use BT's at all

## What are manual transitions used for?
You may want transitions that you only want to take manually/immediately but will never want to repeatedly check/poll a condition for. Ex: Take a transition stateA->stateB when a UI button has been pressed. 
You can just define a manual transition, keep a reference to it and call when the button is pressed.
```csharp
IState stateA;
IState stateB;
StateMachine<IState> fsm;
Button button;
void Start()
{
    var manualTransitionToStateB = fsm.addManualTransitionTo(stateB);
    button.onClick.AddListener(() => {
        fsm.forceTakeTransition(manualTransitionToStateB);
    });
}
```
`fsm.forceTakeTransition(transition)` works for non-manual transitions as well. Use manual transitions when there is no reason to poll a condition.