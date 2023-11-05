namespace BDeshi.BTSM
{
    public class IgnoreStatus : BTSingleDecorator
    {
        private BTStatus statusToOverrideWith;

        public IgnoreStatus(BtNodeBase child, BTStatus statusToOverrideWith = BTStatus.Ignore) : base(child)
        {
            this.statusToOverrideWith = statusToOverrideWith;
        }

        public override void Enter()
        {

        }

        public override BTStatus InternalTick()
        {
            child.Tick();
            return statusToOverrideWith;
        }

        public override void Exit()
        {

        }
        
    }
}