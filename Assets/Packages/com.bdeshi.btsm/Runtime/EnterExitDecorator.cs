using System;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Decorator that invokes callbacks when the child node is entered or exited
    /// </summary>
    public class EnterExitDecorator: BTSingleDecorator
    {
        /// <summary>
        /// called when the child node is entered 
        /// </summary>
        public Action OnEnter;
        /// <summary>
        /// called when the child node is exited
        /// </summary>
        public Action OnExit;

        public EnterExitDecorator(BtNodeBase child, Action onEnter, Action onExit = null) : base(child)
        {
            OnEnter = onEnter;
            OnExit = onExit;
        }

        public override void Enter()
        {
            OnEnter?.Invoke();
            child.Enter();
        }

        public override BTStatus InternalTick()
        {
            return child.Tick();
        }

        public override void Exit()
        {
            OnExit?.Invoke();
            child.Exit();
        }
    }
}