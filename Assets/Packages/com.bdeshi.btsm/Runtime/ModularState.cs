using System;

namespace BDeshi.BTSM
{
    public class ModularState: StateBase
    {
        private Action OnEnter;
        private Action OnTick;

        public ModularState(Action onEnter = null, Action onTick = null)
        {
            OnEnter = onEnter;
            OnTick = onTick;
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
                
        }
    }
}