using System;

namespace BDeshi.BTSM
{
    /// <summary>
    /// A state class with OnEnter tick callbacks
    /// </summary>
    public class ModularState: StateBase
    {
        /// <summary>
        /// Called when EnterState() is called
        /// </summary>
        private Action OnEnter;
        /// <summary>
        /// Called when Tick() is called
        /// </summary>
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