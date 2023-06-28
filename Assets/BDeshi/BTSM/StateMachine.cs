using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// basic Finite State machine class
    /// </summary>
    public class StateMachine
    {
        public State curState;
        public State startingState;
        public GameObject DebugContext = null;

        /// <summary>
        /// Transitions list for this state
        /// </summary>
        public List<Transition> activeTransitions { get; protected set; }

        /// <summary>
        /// This is there in case current state does not have transitions
        /// and so that we don't have to create a new list
        /// </summary>
        public static readonly List<Transition> emptyTransitions =new List<Transition>();
        /// <summary>
        /// Transitions from a state to another
        /// </summary>
        public Dictionary<State, List<Transition>> transitions { get; protected set; } = new Dictionary<State, List<Transition>>();
        /// <summary>
        /// Transitions that are always active
        /// </summary>
        public List<Transition> globalTransitions { get; protected set; }= new List<Transition>();
        public List<Transition> dummyTransition { get; protected set; }= new List<Transition>();
        private State[] states = null;
        
        //hack
        private State[] createAllStatesList()
        {
            HashSet<State> statesHash = new HashSet<State>();
            foreach (var p in transitions)
            {
                statesHash.Add(p.Key);
                foreach (var transition in p.Value)
                {
                    statesHash.Add(transition.SuccessState);
                }
            }

            foreach (var transition in globalTransitions)
            {
                statesHash.Add(transition.SuccessState);
            }

            foreach (var transition in dummyTransition)
            {
                statesHash.Add(transition.SuccessState);
            }

            return statesHash.ToArray();
        }
        public State[] getAllStates()
        {
            if (states == null)
            {
                states = createAllStatesList();
            }

            return states;
        }
        
        
        public StateMachine(State startingState)
        {
            this.startingState = startingState;
        }

        public void enter(bool callEnter = true)
        {
            transitionTo(startingState, callEnter);
        }
        
        public void exitCurState()
        {
            State cur = curState;
            while (cur != null)
            {
                cur.ExitState();
                cur = cur.Parent;
            }
        }

        public void forceTakeTransition(Transition t, bool reEnter = false)
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
                
                foreach (var transition in dummyTransition)
                {
                    transition.TakenLastTime = false;
                }
            #endif
            

            t.OnTaken?.Invoke();
            transitionTo(t.SuccessState, true, reEnter);
            
            //do this after transition 
            //otherwise it would get overwritten if we transitioned to same state
            t.TakenLastTime = true;
        }
        public void Tick()
        {
            curState.Tick();

            State newState = null;
            Transition takenTransition = null;
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
#if DEBUG
                        if (DebugContext)
                            Debug.Log("global transition from " + (curState.FullStateName) + " -> " +  (curState.FullStateName), DebugContext);
#endif
                        break;
                    }
                }
            }
            if(takenTransition != null)
                forceTakeTransition(takenTransition);
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
        public void transitionTo(State newState, bool callEnter = true, bool forceEnterIfSameState = false)
        {
            if (newState != null && (newState != curState || forceEnterIfSameState))
            {

                if (DebugContext)
                    Debug.Log("from " +(curState == null?"null": curState.FullStateName)  + "To " + newState.FullStateName, DebugContext);

                if(callEnter)
                {
                    if (forceEnterIfSameState && newState == curState)
                    {
                        if(curState != null)
                            curState.ExitState();
                        curState = newState;
                        curState.EnterState();
                    }
                    else
                    {
                        // recursiveTransitionToState(newState);
                        if(curState != null)
                            curState.ExitState();
                        curState = newState;
                        curState.EnterState();
                    }
                }
                else
                {
                    curState = newState;
                }
                

                HandleTransitioned();
            }
        }
        /// <summary>
        /// Set transitions list to curState's.
        /// </summary>
        protected virtual void HandleTransitioned()
        {
            if (transitions.TryGetValue(curState, out var newTransitionsList))
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


        void recursiveTransitionToState(State to)
        {
            var cur = curState;

            State commonParent = null;
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
                    toParent = toParent.Parent;
                }
                

                if (commonParent != null)
                    break;
                
                cur.ExitState();
                cur = cur.Parent;
            }
            // Debug.Log( curState?.FullStateName + "->"+ to?.FullStateName+" to "  +commonParent?.FullStateName);
            callEnterRecursive(to, commonParent);
            curState = to;
        }
        /// <summary>
        /// Recurse to some parent and call enterstate of all childs recursively down to the passed one
        /// </summary>
        /// <param name="child"> The child we start recursing from. DO NOT MAKE THIS == PARENT</param>
        /// <param name="limitParent">The parent we won't call enter on. </param>
        void callEnterRecursive(State child, [CanBeNull] State limitParent)
        {
            if(child == null || child == limitParent)
                return;

            callEnterRecursive(child.Parent, limitParent);
            child.EnterState();
        }

        public Transition addTransition(State from, Transition t)
        {
            if(transitions.TryGetValue(from, out var l))
                l.Add(t);
            else
            {
                transitions.Add(from, new List<Transition>(){t});
            }

            return t;
        }
        
        public Transition addTransition(State from, State to, Func<bool> condition, Action onTaken = null )
        {
            return addTransition(from, new SimpleTransition(to, condition, onTaken));
        }

        public StateCondition addStateCondition(bool defaultValue)
        {
            return new StateCondition(this, defaultValue);
        }
        public Transition addGlobalTransition(State to, Func<bool> condition, Action onTaken = null )
        {
            return addGlobalTransition(new SimpleTransition(to, condition, onTaken));
        }
        /// <summary>
        /// fsm never checks these during tick
        /// But can be used via forceTakeTransition()
        /// And also shows up in Editor
        /// </summary>
        /// <param name="to"></param>
        /// <param name="onTaken"></param>
        /// <returns></returns>
        public Transition addDummyTransitionTo(State to,Func<bool> condition = null,  Action onTaken = null )
        {
            var t = new SimpleTransition(to, null, onTaken);
            dummyTransition.Add(t);
            return t;
        }

        public Transition addGlobalTransition(Transition t)
        {
            globalTransitions.Add(t);
            return t;
        }
        

        public void cleanup()
        {
            if (curState != null)
            {
                curState.ExitState();
            }
        }
    }
}