using System;

namespace BDeshi.BTSM
{
    public class ModularState: StateBase
    {
        private Action OnEnter;
        private Action OnTick;
        private Action OnExit;

        public ModularState(Action onEnter = null, Action onTick = null, Action onExit = null)
        {
            OnEnter = onEnter;
            OnTick = onTick;
            OnExit = onExit;
        }

        public override void EnterState()
        {
            OnEnter?.Invoke();
        }

        public override void Tick()
        {
            OnTick?.Invoke();
        }

        public override void ExitState()
        {
            OnExit?.Invoke();
        }
    }
}