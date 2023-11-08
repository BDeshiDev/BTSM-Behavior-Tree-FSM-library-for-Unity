using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace BDeshi.BTSM
{
    public interface IRunnableStateMachine
    {
        /// <summary>
        /// Transitions list for this state
        /// </summary>
        IEnumerable<TransitionBase> ActiveTransitions { get; }

        /// <summary>
        /// Transitions that are always active
        /// </summary>
        IEnumerable<TransitionBase> GlobalTransitions { get; }
        /// <summary>
        /// Transitions that are not evaluated by the fsm
        /// But can be made manually
        /// and will be tracked
        /// You can change states without this
        /// But that won't be shown in the UI
        /// </summary>
        IEnumerable<TransitionBase> ManualTransition { get; }

        IState CurState { get; }
        IState[] getAllStates();
        public GameObject DebugContext { get; set; }
        void enter(bool callEnter = true);
        void Tick();
        void cleanup();

        bool tryGetTransitionsForState(IState state, out IEnumerable<TransitionBase> transitionsForState);
    }

    public class StateMachine<TState> : IRunnableStateMachine
    where TState : class, IState
    {
        public GameObject DebugContext{ get; set; } = null;

        public IState CurState => CurTypedState;
        public TState CurTypedState { get; private set; }

        public TState startingState;


        public IEnumerable<TransitionBase> ActiveTransitions => activeTransitions;

        /// <summary>
        /// Transitions list for this state
        /// </summary>
        public List<Transition<TState>> activeTransitions { get; protected set; }

        /// <summary>
        /// This is there in case current state does not have transitions
        /// and so that we don't have to create a new list
        /// </summary>
        public static readonly List<Transition<TState>> emptyTransitions =new List<Transition<TState>>();
        
        /// <summary>
        /// Transitions from a state to another
        /// </summary>
        public Dictionary<IState, List<Transition<TState>>> transitions = new Dictionary<IState, List<Transition<TState>>>();

        public IEnumerable<TransitionBase> GlobalTransitions => globalTransitions;

        /// <summary>
        /// Transitions that are always active
        /// </summary>
        public List<Transition<TState>> globalTransitions = new List<Transition<TState>>();

        public IEnumerable<TransitionBase> ManualTransition => manualTransitions;
        public List<Transition<TState>> manualTransitions = new List<Transition<TState>>();
        private IState[] states = null;
        public bool ShouldLog = false;

        //hack
        private IState[] createAllStatesList()
        {
            HashSet<IState> statesHash = new HashSet<IState>();
            foreach (var p in transitions)
            {
                statesHash.Add(p.Key);
                foreach (var transition in p.Value)
                {
                    statesHash.Add(transition.SuccessTypedState);
                }
            }

            foreach (var transition in globalTransitions)
            {
                statesHash.Add(transition.SuccessTypedState);
            }

            foreach (var transition in manualTransitions)
            {
                statesHash.Add(transition.SuccessTypedState);
            }

            return statesHash.ToArray();
        }
        public IState[] getAllStates()
        {
            if (states == null)
            {
                states = createAllStatesList();
            }

            return states;
        }
        
        
        public StateMachine(TState startingState)
        {
            this.startingState = startingState;
        }

        public void enter(bool callEnter = true)
        {
            transitionTo(startingState, callEnter);
        }
        
        public void exitCurState()
        {
            IState cur = CurState;
            while (cur != null)
            {
                cur.ExitState();
                cur = cur.Parent;
            }
        }

        public void forceTakeTransition(Transition<TState> t, bool reEnter = false)
        {

            #if UNITY_EDITOR
                foreach (var transition in globalTransitions)
                {
                      transition.TakenLastTime = false;
                }

                foreach (var activeTransition in activeTransitions)
                {
                    activeTransition.TakenLastTime = false;
                }
                
                foreach (var transition in manualTransitions)
                {
                    transition.TakenLastTime = false;
                }
            #endif
            

            t.OnTaken?.Invoke();
            transitionTo(t.SuccessTypedState, true, reEnter);
            
            //do this after transition 
            //otherwise it would get overwritten if we transitioned to same state
            t.TakenLastTime = true;
        }
        public void Tick()
        {
            CurState.Tick();

            IState newState = null;
            Transition<TState> takenTransition = null;
            foreach (var activeTransition in activeTransitions)
            {
                if (activeTransition.Evaluate())
                {
                    takenTransition = activeTransition;
                    break;
                }
            }

            if (newState == null)
            {
                foreach (var activeTransition in globalTransitions)
                {
                    if (activeTransition.Evaluate())
                    {
                        takenTransition = activeTransition;
                        log("global transition from " + (CurState.FullStateName) + " -> " +  (takenTransition.SuccessTypedState.FullStateName));

                        break;
                    }
                }
            }
            if(takenTransition != null)
                forceTakeTransition(takenTransition);
        }

        void log(string text)
        {
            if (ShouldLog)
                Debug.Log(text, DebugContext);
        }

        /// <summary>
        /// Transitions to a state given that it is null
        /// Calls oldstate.exit() if it is not null
        /// Then sets up newState via newState.enter()
        /// Handles recursion.
        /// </summary>
        /// <param name="newState">
        /// Limit this to states this Dictionary knows about. Otherwise, the Actions/Transitions will not work
        /// </param>
        /// <param name="callEnter">
        /// If true, call the enter function in the state(s) transitioned to
        /// Usecase: initialize curState without calling enter
        /// </param>
        /// <param name="forceEnterIfSameState"></param>
        public void transitionTo(TState newState, bool callEnter = true, bool forceEnterIfSameState = false)
        {
            if (newState != null && (newState != CurTypedState || forceEnterIfSameState))
            {
                log((CurState == null?"null": CurState.FullStateName)  + " -> " + newState.FullStateName);

                if(callEnter)
                {
                    if (forceEnterIfSameState && newState == CurState)
                    {
                        if(CurState != null)
                            CurState.ExitState();
                        CurTypedState = newState;
                        CurState.EnterState();
                    }
                    else
                    {
                        // recursiveTransitionToState(newState);
                        if(CurState != null)
                            CurState.ExitState();
                        CurTypedState = newState;
                        CurState.EnterState();
                    }
                }
                else
                {
                    CurTypedState = newState;
                }
                

                HandleTransitioned();
            }
        }
        /// <summary>
        /// Set transitions list to curState's.
        /// </summary>
        protected virtual void HandleTransitioned()
        {
            if (transitions.TryGetValue(CurState, out var newTransitionsList))
            {
                activeTransitions = newTransitionsList;
            }
            else
            {
                activeTransitions = emptyTransitions;
            }
            //need to clear active transition list of new cur state
#if UNITY_EDITOR
            foreach (var activeTransition in activeTransitions)
            {
                activeTransition.TakenLastTime = false;
            }
#endif
        }


        void recursiveTransitionToState(TState to)
        {
            var cur = CurState;

            IState commonParent = null;
            while (cur != null)
            {
                var toParent = to;
                while (toParent != null)
                {
                    if (toParent == cur)
                    {
                        commonParent = cur;
                        break;
                    }
                    toParent = toParent.Parent as TState;
                }
                

                if (commonParent != null)
                    break;
                
                cur.ExitState();
                cur = cur.Parent;
            }
            // Debug.Log( curState?.FullStateName + "->"+ to?.FullStateName+" to "  +commonParent?.FullStateName);
            callEnterRecursive(to, commonParent);
            CurTypedState = to;
        }
        /// <summary>
        /// Recurse to some parent and call enterstate of all childs recursively down to the passed one
        /// </summary>
        /// <param name="child"> The child we start recursing from. DO NOT MAKE THIS == PARENT</param>
        /// <param name="limitParent">The parent we won't call enter on. </param>
        void callEnterRecursive(IState child, [CanBeNull] IState limitParent)
        {
            if(child == null || child == limitParent)
                return;

            callEnterRecursive(child.Parent, limitParent);
            child.EnterState();
        }

        public Transition<TState> addTransition(IState from, Transition<TState> t)
        {
            if(transitions.TryGetValue(from, out var l))
                l.Add(t);
            else
            {
                transitions.Add(from, new List<Transition<TState>>(){t});
            }

            return t;
        }
        
        public Transition<TState> addTransition(TState from, TState to, Func<bool> condition, Action onTaken = null )
        {
            return addTransition(from, new SimpleTransition<TState>(to, condition, onTaken));
        }
        
        public Transition<TState> addGlobalTransition(TState to, Func<bool> condition, Action onTaken = null )
        {
            return addGlobalTransition(new SimpleTransition<TState>(to, condition, onTaken));
        }
        /// <summary>
        /// fsm never checks these during tick
        /// But can be used via forceTakeTransition()
        /// And also shows up in Editor
        /// </summary>
        /// <param name="to"></param>
        /// <param name="onTaken"></param>
        /// <returns></returns>
        public Transition<TState> addManualTransitionTo(TState to, Action onTaken = null )
        {
            var t = new SimpleTransition<TState>(to, null, onTaken);
            manualTransitions.Add(t);
            return t;
        }

        public Transition <TState>addGlobalTransition(Transition<TState> t)
        {
            globalTransitions.Add(t);
            return t;
        }
        

        public void cleanup()
        {
            if (CurState != null)
            {
                CurState.ExitState();
            }
        }

        public bool tryGetTransitionsForState(IState state, out IEnumerable<TransitionBase> transitionsForState)
        {
            if (state is TState tstate)
            {
                if (transitions.TryGetValue(tstate, out var v))
                {
                    transitionsForState = v;
                    return true;
                }
            }

            transitionsForState = default(IEnumerable<TransitionBase>);
            return false;
        }
    }
}