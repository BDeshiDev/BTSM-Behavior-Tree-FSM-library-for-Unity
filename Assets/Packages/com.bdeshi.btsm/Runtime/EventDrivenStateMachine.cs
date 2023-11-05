using System;
using System.Collections.Generic;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Alternate type of FSM that can have a generic type of event and register handlers for them
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class EventDrivenStateMachine<TEvent> : StateMachine
    {
        Dictionary<TEvent, Action> curEventHandlers = new Dictionary<TEvent, Action>();
        Dictionary<TEvent, Action> globalEventHandlers = new Dictionary<TEvent, Action>();
        Dictionary<State, Dictionary<TEvent, Action>> eventHandlers = new Dictionary<State, Dictionary<TEvent, Action>>();

        public static readonly Dictionary<TEvent, Action> emptyEventHandlers = new Dictionary<TEvent, Action>();
        
        public EventDrivenStateMachine(State startingState) : base(startingState) { }

        protected override void HandleTransitioned()
        {
            base.HandleTransitioned();
            if (eventHandlers.TryGetValue(curState, out var handlers))
            {
                curEventHandlers = handlers;
            }
            else
            {
                curEventHandlers = emptyEventHandlers;
            }
        }

        /// <summary>
        /// Call the associated handler in this state, then globals
        /// </summary>
        /// <param name="e">Event</param>
        public void handleEvent(TEvent e)
        {
            if (curEventHandlers.TryGetValue(e, out var handler))
            {
                handler?.Invoke();
                // Debug.Log((curState?.FullStateName)+ " local Handler for event: " + e, DebugContext);
            }else if (globalEventHandlers.TryGetValue(e, out  handler))
            {
                handler?.Invoke();
                // Debug.Log((curState?.FullStateName)+ " global Handler for event: " + e, DebugContext);
            }
            // else
            // {
            //     Debug.Log((curState?.FullStateName)+ " No Handler for event: " + e, DebugContext);
            // }
        }     
        
        /// <summary>
        /// Add global Event Handler under Event e.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="handler">Corresponding action. Can cause state change inside<./param>
        public void addGlobalEventHandler(TEvent e, Action handler)
        {
            globalEventHandlers.Add(e, handler);
        }
        
        /// <summary>
        /// Add Event Handler for state s sunder Event e.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="handler">Corresponding action. Can cause state change inside.</param>
        public void addEventHandler(State s, TEvent e, Action handler)
        {
            Dictionary<TEvent, Action> handlerList;
            
            if (!eventHandlers.TryGetValue(s, out handlerList))
            {
                eventHandlers[s] = new Dictionary<TEvent, Action>();
            }
            

            eventHandlers[s].Add(e, handler);
        }

        public void addEventTransition(State from, TEvent e, State to)
        {
            addEventHandler(from, e, () => transitionTo(to));
        }
        
        public void addGlobalEventTransition(TEvent e, State to)
        {
            addGlobalEventHandler(e, () => transitionTo(to));
        }

    }
}