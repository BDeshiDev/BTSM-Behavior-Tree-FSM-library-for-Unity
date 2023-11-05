using System.Collections.Generic;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Decorator that keeps on trying children one by one until one succeeds
    /// </summary>
    public class FallbackNode : BTMultiDecorator
    {
        [SerializeField] List<IBtNode> children;
        [SerializeField] private int curIndex;
        public override IEnumerable<IBtNode> GetActiveChildren => children;
        public bool retry = true;
        public override void addChild(IBtNode child)
        {
            children.Add(child);
        }

        public FallbackNode(List<IBtNode> children)
        {
            this.children = children;
        }

        public FallbackNode(bool retry = true)
        {
            this.children = new List<IBtNode>();
            this.retry = retry;
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
                return BTStatus.Failure;
            }
            else
            {
                var childResult = children[curIndex].Tick();

                if (childResult == BTStatus.Success)
                    return BTStatus.Success;
                else if (childResult == BTStatus.Failure)
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
    
    /// <summary>
    /// keep on trying earlier children while running lowermost
    /// Earlier in list = higher priority
    /// </summary>
    public class PriorityFallbackNode : BTMultiDecorator
    {
        [SerializeField] List<IBtNode> children;
        [SerializeField] List<bool> hasRun = new List<bool>();
        [SerializeField] private int tryUpTo;
        public override IEnumerable<IBtNode> GetActiveChildren => children;
        public override void addChild(IBtNode child)
        {
            children.Add(child);
            hasRun.Add(false);
        }

        public PriorityFallbackNode(List<IBtNode> children)
        {
            this.children = children;
        }

        public PriorityFallbackNode()
        {
            this.children = new List<IBtNode>();
        }

        public override void Enter()
        {
            tryUpTo = children.Count - 1;
        }


        public override BTStatus InternalTick()
        {
            for (int i = 0; i < children.Count; i++)
            {
                if(!hasRun[i])
                {
                    hasRun[i] = true;
                    children[i].Enter();
                }
                
                var childResult =children[i].Tick();
                if (childResult == BTStatus.Success)
                    return BTStatus.Success;
                if (childResult == BTStatus.Running)
                    break;
            }
            return BTStatus.Running;
        }

        public override void Exit()
        {

            
            for (int i = 0; i < children.Count; i++)
            {
                if(hasRun[i])
                {
                    hasRun[i] = false;
                    children[i].Exit();
                }
            }
        }

    }
}