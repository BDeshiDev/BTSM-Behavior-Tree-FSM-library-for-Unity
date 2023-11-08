using System.Collections.Generic;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Continue running one by one until one fail, then it itself fails.
    /// </summary>
    public class SequenceNode : BTMultiDecorator
    {
        [SerializeField] protected List<IBtNode> children;
        [SerializeField] private int curIndex;
        public override IEnumerable<IBtNode> GetActiveChildren => children;
        public override void addChild(IBtNode child)
        {
            children.Add(child);
        }

        public SequenceNode(List<IBtNode> children)
        {
            this.children = children;
        }
        public SequenceNode()
        {
            this.children = new List<IBtNode>();
        }

        public override void Enter()
        {
            curIndex = 0;
            if (curIndex >= children.Count || curIndex < 0)
            {
                return;
            }
            else
            {
                children[curIndex].Enter();
            }
        }


        public override BTStatus InternalTick()
        {
            if (curIndex >= children.Count || curIndex < 0)
            {
                
                return BTStatus.Success;
            }
            else
            {
                var childResult = children[curIndex].Tick();
                // Debug.Log(curIndex + " "  + childResult);

                if (childResult == BTStatus.Failure)
                    return BTStatus.Failure;
                else if (childResult == BTStatus.Success)
                {
                    children[curIndex].Exit();

                    curIndex++;
                    if (curIndex <= (children.Count - 1))
                    {
                        children[curIndex].Enter();
                    }
                }
            }

            return BTStatus.Running;
        }

        public override void Exit()
        {
            if (curIndex < children.Count  && curIndex >= 0)
            {
                children[curIndex].Exit();
            }
        }

    }
}