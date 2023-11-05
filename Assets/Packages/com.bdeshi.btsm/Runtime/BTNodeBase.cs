using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Base class for decorator BT nodes that can have multiple children
    /// </summary>
    public abstract class BTMultiDecorator: BTDecorator
    {
        public abstract void addChild(IBtNode child);

        public BTMultiDecorator appendChild(IBtNode child)
        {
            addChild(child);
            return this;
        }
    }
    /// <summary>
    /// Base class for decorator BT nodes that can have children
    /// </summary>
    public abstract class BTDecorator : BtNodeBase
    {
        /// <summary>
        /// Get the list of children that would be shown in the editor
        /// </summary>
        public abstract IEnumerable<IBtNode> GetActiveChildren { get; }
    }

    /// <summary>
    /// Base interface for BT nodes
    /// </summary>
    public interface IBtNode
    {
        /// <summary>
        /// Called to initialize the  BTNode before the first tick()
        /// </summary>
        void Enter();
        /// <summary>
        /// Internally called to update the BTNode
        /// Also expected to set the LastStatus
        /// </summary>
        /// <returns> BT Node evaluation status</returns>
        BTStatus Tick();
        /// <summary>
        /// Called when the BTNode is being exited
        /// </summary>
        void Exit();
        /// <summary>
        /// Node Evaluation result from last tick
        /// </summary>
        public BTStatus LastStatus { get; }
        public string Prefix { get; set; } 
        public string Typename => GetType().Name;
        public  string EditorName => Prefix +"_"+ Typename;

    }
    /// <summary>
    /// Base POCO abstract class for BT Nodes
    /// Inherit from this if you don't want monobehavior BT nodes
    /// </summary>
    public abstract class BtNodeBase : IBtNode
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
        /// The tick() method that should be overwritten
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
    /// <summary>
    /// Base abstract class for BT Nodes that are monobehaviors
    /// </summary>
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
        /// <summary>
        /// Is actively running, will block sequence nodes
        /// </summary>
        Running,
        /// <summary>
        /// Succeeded last tick()
        /// </summary>
        Success,
        /// <summary>
        /// Failed last tick()
        /// </summary>
        Failure,
        /// <summary>
        /// Neither success nor failure, non blocking running
        /// Ex Use case: Parallel node where you want to keep running child regardless of what others do 
        /// </summary>
        Ignore,
        /// <summary>
        /// the node has not been called yet
        /// DO NOT RETURN THIS ON TICK()
        /// </summary>
        NotRunYet,
    }
}
