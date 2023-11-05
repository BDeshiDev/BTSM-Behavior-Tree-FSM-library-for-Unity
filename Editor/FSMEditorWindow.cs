using BDeshi.BTSM;
using UnityEditor;
using UnityEngine;

namespace BDeshi.BTSM.Editor
{
    public class FSMEditorWindow : EditorWindow
    {
        public FSMRunner runner;
        private const int maxRecursion = 128;
        public State selectedState = null;
        private bool syncToCurState = true;
        private Color oldCol;
        private GUIStyle curStateTrackerStyle;
        #region guistate
        private Vector2 stateListScrollPos;
        private Vector2 curStateScrollPos;
        private bool showCurStatesToggle = true;
        private bool showAllStateToggle = true;
        private int StatesListwidth = 200;
        private int transitionsListWidth = 400;
        private Vector2 SelectedStateBTScrollPosition;
        private Vector2 curStateBTScrollPosition;
        private Vector2 curStateTransitionScollPos;
        private Vector2 selectedStateTransitionScollPos;
        #endregion

        #region Runnner management

        void OnSelectionChange()
        {
            getRunnner();
        }

        private void getRunnner()
        {
            if (Selection.activeTransform != null)
            {
                var _fsmRunner = Selection.activeTransform.GetComponent<FSMRunner>();
                if (_fsmRunner != null)
                {
                    runner = _fsmRunner;
                }
            }
        }

        void refreshRunner()
        {
            if (runner == null)
            {
                getRunnner();
            }
        }

        #endregion

        #region draw BT
        /// <summary>
        /// Draw BT if state is BTWrapperState
        /// </summary>
        /// <param name="s"></param>
        void drawBT(BTWrapperState s)
        {
            if (s != null)
            {
                drawNode(s.BTRoot);
            }
        }



        private Color getNodeColor(BTStatus status)
        {
            switch (status)
            {
                case BTStatus.Running:
                    return Color.yellow;
                case BTStatus.Success:
                    return Color.green;
                case BTStatus.Failure:
                    return Color.red;
                case BTStatus.Ignore:
                    return Color.blue;

            }

            return Color.white;
        }


        void drawNode(IBtNode n, int depth = 0)
        {
            if (n == null)
                return;
            depth++;
            if (depth >= maxRecursion)
                return;
            EditorGUILayout.LabelField(n.EditorName);
            GUI.color = getNodeColor(n.LastStatus);

            if (n is BTDecorator decorator)
            {
                EditorGUI.indentLevel++;
                foreach (var child in decorator.GetActiveChildren)
                {
                    drawNode(child, depth);
                }
                EditorGUI.indentLevel--;

            }
        }


        #endregion

        [MenuItem("Window/BTSM")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            FSMEditorWindow window = (FSMEditorWindow)EditorWindow.GetWindow(typeof(FSMEditorWindow));
            window.Show();


        }


        void OnInspectorUpdate()
        {
            // Call Repaint on OnInspectorUpdate as it repaints the windows
            // less times as if it was OnGUI/Update
            if (Application.isPlaying)
            {
                refreshRunner();
            }
            Repaint();
        }
        private void OnGUI()
        {
            if (runner == null)
            {
                EditorGUILayout.HelpBox("No fsmRunner selected", MessageType.Warning);
            }
            else
            {

                var fsm = runner.fsm;
                if (fsm == null)
                {
                    EditorGUILayout.HelpBox("No fsm", MessageType.Error);
                    return;
                }

                oldCol = GUI.color;

                var colStyle = new GUIStyle();
                colStyle.normal.background = Texture2D.grayTexture;
                using (var horzGroupScope = new EditorGUILayout.HorizontalScope())
                {
                    using (var verticalGroupScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(StatesListwidth)))
                    {
                        if (GUILayout.Button("Select fsm owner"))
                        {
                            Selection.activeGameObject = runner.gameObject;
                        }

                        curStateTrackerStyle = new GUIStyle("Button");
                        curStateTrackerStyle.normal.textColor = Color.red;
                        syncToCurState = GUILayout.Toggle(syncToCurState,
                            (syncToCurState ? "tracking Cur state" : "not tracking Cur state"),
                            curStateTrackerStyle);

                        drawCurState(fsm);
                        drawAllStatesList(fsm);
                    }
                    using (var verticalGroupScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(transitionsListWidth)))
                    {
                        drawStateTransitions(fsm);
                        GUI.color = oldCol;

                        drawGlobalTransitions(fsm);
                        GUI.color = oldCol;
                        drawDummyTransitions(fsm);
                        GUI.color = oldCol;
                    }

                    drawStateBT(fsm);
                }

            }


        }

        #region drawState
        const int maxStateDrawDepthLimit = 5;
        void drawStateSideBarPreview(State s, int depth = 0)
        {
            if (s == null)
                return;

            drawStateLabel(s, depth);

            if (depth++ > maxStateDrawDepthLimit)
            {
                return;
            }
            if (s is ContainerState p)
            {
                EditorGUI.indentLevel++;
                foreach (var child in p.getChildStates())
                {
                    drawStateSideBarPreview(child, depth);
                }
                EditorGUI.indentLevel--;

            }
        }

        private void drawStateLabel(State s, int depth = 0)
        {
            var isSelectedState = s == selectedState;
            var color = GUI.color;
            if (isSelectedState)
            {
                GUI.color = syncToCurState ? Color.yellow : Color.green;
            }

            var name = (s.Name).PadLeft(s.Name.Length + depth * 2, '-');
            if (EditorGUILayout.LinkButton(name))
            {
                syncToCurState = false;
                selectedState = s;
                if (s is MonoBehaviorState ms)
                {
                    Selection.activeGameObject = ms.gameObject;
                }
            }

            if (isSelectedState)
            {
                GUI.color = color;
            }
        }
        void drawTransition(Transition transition)
        {
            GUI.color = transition.TakenLastTime ? Color.green : Color.red;
            EditorGUILayout.LabelField("-->" + (transition.SuccessState == null ? "?" : transition.SuccessState.FullStateName));
        }
        private void drawStateBT(StateMachine sm)
        {
            if (syncToCurState || selectedState == null)
            {
                drawStateBT(sm.curState);
            }
            else
            {
                drawStateBT(selectedState);
            }
        }
        private void drawStateBT(State state)
        {
            if (state != null && state is BTWrapperState curBT)
            {
                using (var verticalGroupScope = new EditorGUILayout.ScrollViewScope(curStateBTScrollPosition))
                {
                    EditorGUILayout.LabelField("Cur State BT", EditorStyles.whiteLargeLabel);
                    curStateBTScrollPosition = verticalGroupScope.scrollPosition;
                    drawBT(curBT);
                }
            }
        }

        #endregion



        private void drawCurState(StateMachine fsm)
        {
            showCurStatesToggle = EditorGUILayout.BeginFoldoutHeaderGroup(showCurStatesToggle, "CurState:");
            if (showCurStatesToggle)
            {
                using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(curStateScrollPos))
                {
                    curStateScrollPos = scrollViewScope.scrollPosition;

                    drawStateLabel(fsm.curState);
                    GUI.color = oldCol;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void drawAllStatesList(StateMachine fsm)
        {
            showAllStateToggle = EditorGUILayout.BeginFoldoutHeaderGroup(showAllStateToggle, "All States:");
            if (showAllStateToggle)
            {
                using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(stateListScrollPos))
                {
                    stateListScrollPos = scrollViewScope.scrollPosition;

                    foreach (var s in fsm.getAllStates())
                    {
                        drawStateSideBarPreview(s);
                        GUI.color = oldCol;
                    }

                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }



        private void drawStateTransitions(StateMachine fsm)
        {
            if ((syncToCurState || selectedState == null) && fsm.curState != null)
            {
                drawStateTransitions(fsm, "ActiveState", fsm.curState);

            }
            else
            {
                drawStateTransitions(fsm, "SelectedState", selectedState);
            }

        }

        private void drawStateTransitions(StateMachine fsm, string prefix, State state)
        {
            GUILayout.Label(prefix + "(" + state.FullStateName + ") Transitions:", EditorStyles.whiteLargeLabel);
            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(selectedStateTransitionScollPos))
            {
                selectedStateTransitionScollPos = scrollViewScope.scrollPosition;
                if (fsm.transitions.ContainsKey(state))
                {
                    foreach (var transition in fsm.transitions[state])
                    {
                        drawTransition(transition);
                    }
                }
            }
        }

        private void drawDummyTransitions(StateMachine fsm)
        {
            GUILayout.Label("Dummy Transitions:", EditorStyles.whiteLargeLabel);
            foreach (var transition in fsm.dummyTransition)
            {
                drawTransition(transition);
            }
        }

        private void drawGlobalTransitions(StateMachine fsm)
        {
            GUILayout.Label("global Transitions:", EditorStyles.whiteLargeLabel);

            foreach (var transition in fsm.globalTransitions)
            {
                drawTransition(transition);
            }
        }
    }
}
