using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Repeats child regardless of result, itself returning running
    /// After N times, return success
    /// If n < 0, keep ticking,
    /// </summary>
    public class Repeat:BTSingleDecorator
    {
        private int n = 0;
        private int c = 0;

        public Repeat(int n, BtNodeBase child) : base(child)
        {
            this.n = n;
        }

        
        public Repeat(BtNodeBase child) : base(child)
        {
            this.n = -1;
        }

        public override void Enter()
        {
            c = n;
            child.Enter();
        }

        public override BTStatus InternalTick()
        {
            if (c == 0)
            {
                return BTStatus.Success;
            }

            var r = child.Tick();
            if (r == BTStatus.Success || r == BTStatus.Failure)
            {
                
                child.Exit();

                if (c > 0)
                {
                    c--;
                    if (c != 0)
                    {
                        child.Enter();
                    }
                }
                else 
                {
                    child.Enter();
                }

                return BTStatus.Running;
            }

            return BTStatus.Running;
        }

        public override void Exit()
        {
            
        }
    }
}