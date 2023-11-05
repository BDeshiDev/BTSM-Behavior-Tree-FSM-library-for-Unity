using System;

namespace BDeshi.BTSM
{
    public abstract class StateBase: IState
    {
        public abstract void EnterState();
        public abstract void Tick();
        public abstract void ExitState();
        public string Prefix { get; set; }
        public string FullStateName => Prefix +"_"+ GetParentChainName();
        public IState Parent { get; set; }
               
        public string Name => this.GetType().Name;

        public IState AsChildOf(IState p)
        {
            Parent = p;
            return this;
        }

        public String GetParentChainName()
        {
            string chain = Name;
            var p = Parent;
            while (p != null)
            {
                chain = p.Name + "." + chain;
                p = p.Parent;
            }
            return chain;
        }
        
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}