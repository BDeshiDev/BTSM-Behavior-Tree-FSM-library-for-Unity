using System.Collections.Generic;
using Bdeshi.Helpers.Utility.Extensions;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// Run all children every tick
    /// Succeed on first on succeeding, or when all do.
    /// If the later, the children may or may not restart
    /// </summary>
    public class Parallel: BTMultiDecorator
    {
        public override IEnumerable<IBtNode> GetActiveChildren => children;
        private List<IBtNode> children;
        private int runThreshold;
        private bool allMustSucceed;
        /// <summary>
        /// Will it restart children that have succeded if allMustSucceed = true?
        /// </summary>
        private bool repeatSuccessfullChildren;
        

        public Parallel(List<IBtNode> activeChildren, bool allMustSucceed =false, bool repeatSuccessfullChildren =false)
        {
            this.children = activeChildren;
            this.allMustSucceed = allMustSucceed;
            this.repeatSuccessfullChildren = repeatSuccessfullChildren;
        }
        
        public Parallel(bool allMustSucceed =false, bool repeatSuccessfullChildren = false)
            : this(new List<IBtNode>(), allMustSucceed, repeatSuccessfullChildren)
        {
    
        }


        public override void Enter()
        {
            runThreshold = children.Count;

            foreach (var btNodeBase in children)
            {
                btNodeBase.Enter();
            }
        }

        public override BTStatus InternalTick()
        {

            bool anySuccess = false;
            bool allSuccess = true;
            for(int i = runThreshold - 1; i >= 0; i--)
            {
                var btNodeBase = children[i];
                var status = btNodeBase.Tick();
                if (status== BTStatus.Success)
                {
                    anySuccess = true;
                    if (allMustSucceed)
                    {
                        if (repeatSuccessfullChildren)
                        {
                            children[i].Exit();
                            children[i].Enter();
                        }else
                        {
                            // Debug.Log(children[i].Typename + " complete " + i + " " + runThreshold + children[i].LastStatus);

                            children.swapToLast(i);
                            runThreshold--;

                            // Debug.Log(children[children.Count -1 ].Typename + "afterwards complete " + (children.Count - 1) + " " + runThreshold + children[children.Count -1 ].LastStatus);
                            
                        }
                    }

                }
                else if(status != BTStatus.Ignore)
                {
                    allSuccess = false;
                }
            }

            // foreach (var child in children)
            // {
            //     Debug.Log(child.Typename + " " + child.LastStatus);
            // }

            if ((allMustSucceed && allSuccess) || (!allMustSucceed && anySuccess))
                return BTStatus.Success;
            return BTStatus.Running;
        }


        public override void Exit()
        {
            foreach (var btNodeBase in children)
            {
                btNodeBase.Exit();
            }
        }

        public override void addChild(IBtNode child)
        {
            children.Add(child);
        }
    }
    
    /// <summary>
    /// Run all children every tick
    /// Succeed on first on succeeding, or when all do.
    /// If the later, the children may or may not restart
    /// </summary>
    public class ParallelRepeat: BTMultiDecorator
    {
        public override IEnumerable<IBtNode> GetActiveChildren => children;
        private List<IBtNode> children;
        private int runThreshold;
        public BTStatus runninStatus = BTStatus.Running;
        

        public ParallelRepeat(List<IBtNode> activeChildren)
        {
            this.children = activeChildren;
        }
        

        public override void Enter()
        {
            runThreshold = children.Count;

            foreach (var btNodeBase in children)
            {
                btNodeBase.Enter();
            }
        }

        public override BTStatus InternalTick()
        {
            for(int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                var status = child.Tick();
                if (status== BTStatus.Success || status == BTStatus.Failure)
                {
                    child.Exit();
                    child.Enter();
                }
            }

            return BTStatus.Running;
        }


        public override void Exit()
        {
            foreach (var btNodeBase in children)
            {
                btNodeBase.Exit();
            }
        }

        public override void addChild(IBtNode child)
        {
            children.Add(child);
        }
    }
}