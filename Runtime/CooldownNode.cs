using System.Collections.Generic;
using UnityEngine;

namespace BDeshi.BTSM
{
    /// <summary>
    /// prevent running child for x secs after it has run || it started running
    /// </summary>
    public class CooldownNode : BTSingleDecorator
    {
        public float waitStart;
        public float waitDuration = 5;

        private bool isRunning;
        
        private CoolDownResetType resetType;
        public BTStatus cooldownFailStatus = BTStatus.Failure;
        public enum CoolDownResetType
        {
            ResetOnStart,
            ResetOnExit,
        }

        public override void Enter()
        {
            isRunning = false;
            // if(resetType == CoolDownResetType.ResetOnStart)
            //     waitStart = Time.time;
            
            if(lastStatus == BTStatus.NotRunYet)
                waitStart = Time.time;
        }

        public void startRunning()
        {
            Debug.Log(" really start runnning");
            isRunning = true;
            // waitStart = Time.time;
            child.Enter();
        }


        public override BTStatus InternalTick()
        {
            Debug.Log(isRunning);
            if (isRunning)
            {
                Debug.Log("is runnning");
                var childStatus = child.Tick();
                if (childStatus == BTStatus.Success)
                {
                    waitStart = Time.time;
                    isRunning = false;
                }
                return childStatus;
            }else if((Time.time - waitStart) >= waitDuration)
            {
                Debug.Log("start runnning");
                if (child == null){

                    return BTStatus.Success;
                }
                else
                {
                    startRunning();
                    var childStatus = child.Tick();
                    if (childStatus == BTStatus.Success)
                    {
                        waitStart = Time.time;
                        isRunning = false;
                    }
                        
                    return childStatus;
                }
            }
            else
            {
                Debug.Log("fail");
                return cooldownFailStatus;
            }
        }

        public override void Exit()
        {
            if(isRunning)
            {
                child.Exit();
            }
            // if (resetType == CoolDownResetType.ResetOnExit)
            //     waitStart = Time.time;
            
        }

        public override string EditorName => $"{base.EditorName} [{waitDuration - (Time.time - waitStart)}] left";

        public CooldownNode(BtNodeBase child, float waitDuration, BTStatus cooldownFailStatus = BTStatus.Failure, CoolDownResetType resetType = CoolDownResetType.ResetOnExit, bool shouldWaitAtStart = false) : base(child)
        {
            this.waitDuration = waitDuration;
            this.waitStart = Time.time;
            this.resetType = resetType;
            this.cooldownFailStatus = cooldownFailStatus;
            this.isRunning = !shouldWaitAtStart;
        }
    }
}