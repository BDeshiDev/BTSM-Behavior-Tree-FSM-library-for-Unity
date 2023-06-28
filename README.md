# BTSM-Behavior-Tree-FSM-library-for-Unity
BTSM is a Behavior Tree + Finite State Machine library I wrote for Unity.
- States, BT and Transitions are modular and are written entirely in code
- States can contain behavior trees.
- Supports Hierarchical Finite State Machines. States can contain child states.
- Can transition between states depending on func/lambda based conditions.
- Sequence, Parallel etc. common decorators are provided.
- Runtime debugging via custom editor window.
- Since it's fully code based, you can use your IDE to debug, refactor and reuse BT/States easily
- States and BT classes must either be plain C# classes or Monobehaviors. 
# Code
[You can see the main classes here](https://github.com/BDeshiDev/BTSM-Behavior-Tree-FSM-library-for-Unity/tree/main/Assets/BDeshi/BTSM) 
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
