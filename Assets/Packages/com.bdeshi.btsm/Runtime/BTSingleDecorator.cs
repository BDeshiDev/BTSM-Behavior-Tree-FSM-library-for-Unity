using System.Collections.Generic;

namespace BDeshi.BTSM
{
    public abstract class BTSingleDecorator : BTDecorator
    {
        /// <summary>
        /// The single child BTNode of this decorator
        /// </summary>
        protected IBtNode child;
        public override  IEnumerable<IBtNode> GetActiveChildren => getChildWrapper();

        IEnumerable<IBtNode> getChildWrapper()
        {
            yield return child;
        }

        protected BTSingleDecorator(IBtNode child)
        {
            this.child = child;
        }
    }
}