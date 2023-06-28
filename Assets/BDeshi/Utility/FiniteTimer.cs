using UnityEngine;

namespace BDeshi.Utility
{
    [System.Serializable]
    public struct FiniteTimer
    {
        public float timer;
        public float maxValue;

        public void init(float maxval, float startVal = 0)
        {
            timer = startVal;
            maxValue = maxval;
        }

        public FiniteTimer(float timerStart, float maxVal, bool completed = false)
        {
            timer = completed ? maxVal : timerStart;
            maxValue = maxVal;
        }

        public FiniteTimer(float maxVal = 3, bool completed = false)
        {
            timer = completed ? maxVal : 0;
            maxValue = maxVal;
        }




        public void updateTimer(float delta)
        {
            timer += delta;
        }
        /// <summary>
        /// return true if this is completed before or after updating
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool tryCompleteTimer(float delta)
        {
            return tryCompleteTimer(delta, out var r);
        }
        
        /// <summary>
        /// return true if this is completed after updating only if it wasn't completed before
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool tryCompleteOnce(float delta)
        {
            if (isComplete)
            {
                return false;
            }
            return tryCompleteTimer(delta, out var r);
        }

        public bool tryCompleteTimer(float delta, out float remainder)
        {
            remainder = delta;
            if (isComplete)
                return true;
            timer += delta;
            if (timer > maxValue)
            {
                remainder = timer - maxValue;
                return true;
            }

            remainder = 0;
            return false;
        }
        
        
        public bool tryEmptyTimer(float delta, out float remainder)
        {
            remainder = delta;
            if (timer <= 0)
                return true;

            if (timer <= delta)
            {
                timer = 0;
                remainder -= timer;
                return true;
            }
            else
            {
                timer -= remainder;
                remainder = 0;
            }
            return false;
        }

        public void clampedUpdateTimer(float delta)
        {
            timer = Mathf.Clamp(timer + delta, 0, maxValue);
        }

        public void safeUpdateTimer(float delta)
        {
            timer = Mathf.Clamp(timer + delta, 0, maxValue);
        }
        
        public void safeSubtractTimer(float delta)
        {
            timer = Mathf.Clamp(timer - delta, 0, maxValue);
        }


        public void reset()
        {
            timer = 0;
        }

        public void resetByFractionOfMax(float frac)
        {
            timer = Mathf.Max(0, timer - frac * maxValue);
        }
        
        /// <summary>
        /// Reset and set max
        /// </summary>
        /// <param name="newMax"></param>
        public void reset(float newMax)
        {
            maxValue = newMax;
            reset();
        }
        
        public void resetAndSetToMax(float newMax)
        {
            timer = maxValue = newMax;
        }

        public void resetAndKeepExtra()
        {
            if (timer > maxValue)
                timer -= maxValue;
            else
                reset();
        }

        public void complete()
        {
            timer = maxValue;
        }

        public bool isComplete => timer >= maxValue;

        public bool exceedsRatio(float ratioToExceed)
        {
            return Ratio >= ratioToExceed;
        }

        public float Ratio => Mathf.Clamp01(timer / maxValue);

        public float ReverseRatio => 1 - Ratio;
    }
    
    [System.Serializable]
    public class SafeFiniteTimer
    {
        public float timer;
        public float maxValue;

        public void init(float maxval, float startVal = 0)
        {
            timer = startVal;
            maxValue = maxval;
        }

        public SafeFiniteTimer(float timerStart, float maxVal, bool completed = false)
        {
            timer = completed ? maxVal : timerStart;
            maxValue = maxVal;
        }

        public SafeFiniteTimer(float maxVal = 3, bool completed = false)
        {
            timer = completed ? maxVal : 0;
            maxValue = maxVal;
        }




        public void updateTimer(float delta)
        {
            timer += delta;
        }
        /// <summary>
        /// return true if this is completed before or after updating
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool tryCompleteTimer(float delta)
        {
            return tryCompleteTimer(delta, out var r);
        }

        public bool tryCompleteTimer(float delta, out float remainder)
        {
            remainder = delta;
            if (isComplete)
                return true;
            timer += delta;
            if (timer > maxValue)
            {
                remainder = timer - maxValue;
                return true;
            }

            remainder = 0;
            return false;
        }
        
        
        public bool tryEmptyTimer(float delta, out float remainder)
        {
            remainder = delta;
            if (timer <= 0)
                return true;

            if (timer <= delta)
            {
                timer = 0;
                remainder -= timer;
                return true;
            }
            else
            {
                timer -= remainder;
                remainder = 0;
            }
            return false;
        }

        public void clampedUpdateTimer(float delta)
        {
            timer = Mathf.Clamp(timer + delta, 0, maxValue);
        }

        public void safeUpdateTimer(float delta)
        {
            if (timer < maxValue)
                timer += delta;
        }

        public void reset()
        {
            timer = 0;
        }

        public void resetByFractionOfMax(float frac)
        {
            timer = Mathf.Max(0, timer - frac * maxValue);
        }
        
        /// <summary>
        /// Reset and set max
        /// </summary>
        /// <param name="newMax"></param>
        public void reset(float newMax)
        {
            maxValue = newMax;
            reset();
        }

        public void resetAndKeepExtra()
        {
            if (timer > maxValue)
                timer -= maxValue;
            else
                reset();
        }

        public void complete()
        {
            timer = maxValue;
        }

        public bool isComplete => timer >= maxValue;

        public bool exceedsRatio(float ratioToExceed)
        {
            return Ratio >= ratioToExceed;
        }

        public float Ratio => Mathf.Clamp01(timer / maxValue);

        public float ReverseRatio => 1 - Ratio;
    }
}