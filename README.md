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