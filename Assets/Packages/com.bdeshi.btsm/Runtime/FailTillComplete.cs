namespace BDeshi.BTSM
{
    /// <summary>
    /// Decorator that returns failure until child succeeds
    /// </summary>
    public class FailTillComplete : BTSingleDecorator
    {
        public override void Enter()
        {
            
        }

        public override BTStatus InternalTick()
        {
            var result = child.Tick();
            return result == BTStatus.Success ? BTStatus.Failure : BTStatus.Success;
        }

        public override void Exit()
        {
            
        }

        public FailTillComplete(BtNodeBase child) : base(child)
        {
        }
    }
}