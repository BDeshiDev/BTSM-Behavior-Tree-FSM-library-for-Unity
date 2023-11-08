using System;

namespace BDeshi.BTSM
{
    public class EnterExitDecorator: BTSingleDecorator
    {
        public Action OnEnter;
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