using System.Collections.Generic;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// prevent running child for x secs after it has run or it started running
    /// </summary>
    public class CooldownNode : BTSingleDecorator
    {
        public float waitStart;
        public float waitDuration = 5;

        private bool isRunning;
        
        private CoolDownType type;
        public enum CoolDownType
        {
            ResetOnStart,
            ResetOnExit,
        }

        public override void Enter()
        {
            isRunning = false;
            if(type == CoolDownType.ResetOnStart)
                waitStart = Time.time;
        }

        public void startRunning()
        {
            isRunning = true;
            child.Enter();
        }


        public override BTStatus InternalTick()
        {
            if (isRunning)
            {
                return child.Tick();
            }else if((Time.time - waitStart) >= waitDuration)
            {
                if (child == null)
                    return BTStatus.Success;
                else
                {
                    startRunning();
                    return child.Tick();
                }
            }
            else
            {
                return BTStatus.Failure;
            }
        }

        public override void Exit()
        {
            if(isRunning)
            {
                child.Exit();
                if (type == CoolDownType.ResetOnExit)
                    waitStart = Time.time;
            }
        }

        public override string EditorName => $"{base.EditorName} [{waitDuration - (Time.time - waitStart)}] left";

        public CooldownNode(BtNodeBase child, float waitDuration, CoolDownType type = CoolDownType.ResetOnExit, bool shouldWaitAtStart = false) : base(child)
        {
            this.waitDuration = waitDuration;
            this.waitStart = Time.time;
            this.type = type;
            this.isRunning = !shouldWaitAtStart;
        }
    }
}