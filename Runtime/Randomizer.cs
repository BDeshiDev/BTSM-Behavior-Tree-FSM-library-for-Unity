using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace BDeshi.BTSM
{
    public class Randomizer : BTMultiDecorator
    {
        private List<IBtNode> children = new List<IBtNode>();
        private List<float> weights = new List<float>();
        private List<Func<bool>> availabilityFunc;
        private int pickIndex = 0;
        
        // #TODO add random weight readjustment when a child is picked
        

        public int pick()
        {
            float randomVal = Random.Range(0, calcTotalWeight());
            float runningSum = 0;
            for (pickIndex = 0; pickIndex < children.Count && (weights[pickIndex] + runningSum)  < randomVal; pickIndex++)
            {
                runningSum += weights[pickIndex];
            }

            return pickIndex;
        }

        public float calcTotalWeight()
        {
            float sum = 0;
            foreach (var weight in weights)
            {
                sum += weight;
            }

            return sum;
        }

        public override void Enter()
        {
            pick();
            children[pickIndex].Enter();
        }

        public override BTStatus InternalTick()
        {
            return children[pickIndex].Tick();
        }

        public override void Exit()
        {
            children[pickIndex].Tick();
        }

        public override IEnumerable<IBtNode> GetActiveChildren => children;
        public override void addChild(IBtNode child)
        {
            children.Add(child);
            weights.Add(1);
        }
        
        public virtual Randomizer appendChild(float weight, IBtNode child)
        {
            children.Add(child);
            weights.Add(weight);
            return this;
        }
    }
}