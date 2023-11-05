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
To use:
1. Add a FSMRunner component.
2. Create a FSM, add states and transitions(can be done in start())
3. Use a BTWrapperState if you want behavior trees. The BT will be run through this state.
4. Pass fsm object to FSMRunner via initialize()
5. Open the FSMEditorwindow.
6. Select the Gameobject with the FSMRunner.
7. The Editor window will work during playmode.

I mostly made this for myself but I will add a demo scene if there is interest. The project I'm using this in would be hard to use as an independent demo project.

# Manual
https://bdeshidev.github.io/BTSM-Behavior-Tree-FSM-library-for-Unity/manual/
