using System;
using System.Collections;
using System.Collections.Generic;
using Bdeshi.Helpers.Utility;
using UnityEngine;

namespace BDeshi.BTSM
{
    public abstract class BTMultiDecorator: BTDecorator
    {
        public abstract void addChild(IBtNode child);

        public BTMultiDecorator appendChild(IBtNode child)
        {
            addChild(child);
            return this;
        }
    }

    public abstract class BTDecorator : BtNodeBase
    {
        public abstract IEnumerable<IBtNode> GetActiveChildren { get; }
    }
    
    public class ConditionNode:BtNodeBase
    {
        private Func<bool> func;
        public BTStatus successState;
        public BTStatus failState;

        public ConditionNode(Func<bool> func, BTStatus successState = BTStatus.Success, BTStatus failState = BTStatus.Failure)
        {
            this.func = func;
            this.successState = successState;
            this.failState = failState;
        }

        public override void Enter( )
        {
            
        }

        public override void Exit()
        {
            
        }

        public override BTStatus InternalTick()
        {
            return func() ? successState : failState;
        }
    }
    
    public class MaintainConditionNode:BtNodeBase
    {
        private Func<bool> func;
        public BTStatus successState;
        public BTStatus waitState;
        public bool resetOnFail;
        public FiniteTimer maintainTimer;

        public MaintainConditionNode(Func<bool> func, float maintainTime, bool resetOnFail = true, BTStatus successState = BTStatus.Success, BTStatus waitState = BTStatus.Running)
        {
            this.func = func;
            this.successState = successState;
            this.waitState = waitState;
            this.maintainTimer = new FiniteTimer(maintainTime);
            this.resetOnFail = resetOnFail;
        }

        public override void Enter( )
        {
            maintainTimer.reset();
        }

        public override void Exit()
        {
            
        }

        public override BTStatus InternalTick()
        {
            bool success = func();
            if (success)
            {
                if (maintainTimer.tryCompleteTimer(Time.deltaTime))
                {
                    return successState;
                }
                else
                {
                    return BTStatus.Running;
                }
            }
            else
            {
                if (resetOnFail)
                {
                    maintainTimer.reset();
                    
                }

                return waitState;
            }
        }
    }

    public class TimerNode:BTSingleDecorator
    {
        public FiniteTimer duration;
        public BTStatus timeoutStatus = BTStatus.Success;

    
        public override string EditorName => $"{base.EditorName} [{duration.remaingValue()}] left";
        public override void Enter()
        {
            duration.reset();
            child.Enter();   
        }

        public override void Exit()
        {
            child.Exit();
        }

        public override BTStatus InternalTick()
        {
            duration.updateTimer(Time.deltaTime);
            if (duration.isComplete)
            {
                return timeoutStatus;
            }
            else
            {

    
                return child.Tick();
            }
        }

        public TimerNode(IBtNode child, float timeDuration) : base(child)
        {
            this.duration = new FiniteTimer(timeDuration);
        }

    }

    public interface IBtNode
    {
        void Enter();
        
        BTStatus Tick();
        void Exit();

        public BTStatus LastStatus { get; }

        public string Prefix { get; set; } 
        public string Typename => GetType().Name;
        public  string EditorName => Prefix +"_"+ Typename;

    }

    public abstract class BtNodeBase : IBtNode
    {
        public abstract void Enter();

        /// <summary>
        /// NOT VIRTUAL OR ABSTRACT. DO NOT OVERRIDE.
        /// Override internal tick instead.
        /// This just calls InternalTick() and saves the result onto lastStatus
        /// </summary>
        /// <returns></returns>
        public BTStatus Tick()
        {
            lastStatus = InternalTick();
            return lastStatus;
        }

        /// <summary>
        /// Called everytime the node is ticked
        /// Override this for subclass nodes
        /// </summary>
        /// <returns>The result from ticking </returns>
        public abstract BTStatus InternalTick();
        /// <summary>
        /// Result from last tick
        /// </summary>
        public BTStatus LastStatus => lastStatus;
        public abstract void Exit();
        public string Prefix { get; set; }

        protected BTStatus lastStatus = BTStatus.NotRunYet;
        public string Typename => GetType().Name;
        public virtual string EditorName => Prefix +"_"+ Typename;
    }
    
    public abstract class BtNodeMonoBase : MonoBehaviour,IBtNode
    {
        public abstract void Enter();

        /// <summary>
        /// NOT VIRTUAL OR ABSTRACT. DO NOT OVERRIDE.
        /// Override internal tick instead.
        /// This approach is for making tracking status is easier
        /// </summary>
        /// <returns></returns>
        public BTStatus Tick()
        {
            lastStatus = InternalTick();
            return lastStatus;
        }

        /// <summary>
        /// To allow caching status onto lastStatus
        /// </summary>
        /// <returns></returns>
        public abstract BTStatus InternalTick();

        public BTStatus LastStatus => lastStatus;
        public abstract void Exit();
        public string Prefix { get; set; }

        protected BTStatus lastStatus = BTStatus.NotRunYet;
        public string Typename => GetType().Name;
        public virtual string EditorName => Prefix +"_"+ Typename;
    }

    public enum BTStatus{
        NotRunYet,
        /// <summary>
        /// Is actively running, will block sequence nodes
        /// </summary>
        Running,
        Success,
        Failure,
        /// <summary>
        /// Neither success nor failure, non blocking running
        /// Ex Use case: Parallel node where you want to keep running child regardless of what others do 
        /// </summary>
        Ignore,

    }
}
