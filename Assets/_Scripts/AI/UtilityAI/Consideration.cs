using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public abstract class Consideration : ScriptableObject
    {
        public string Name;
        [SerializeField] private AnimationCurve curve;

        [SerializeField] protected float maxValue = 1.0f;

        /// <summary>
        /// An exclusive consideration HAS to be greater than 0 so that the action in whole is valid
        /// </summary>
        public bool isExclusiveConsideration = false;
        /// <summary>
        /// Overrides Curve with: 0.0f -> 0.0f; else 1.0f
        /// </summary>
        public bool zeroStepCurve = false;
        /// <summary>
        /// Overrides Curve with: 1.0f -> 1.0f; else 0.0f
        /// </summary>
        public bool oneStepCurve = false;

        public abstract float ScoreConsideration(Action action, UtilityAIController controller, Object[] instanceData);

        public virtual float Evaluate(float input)
        {
            if(zeroStepCurve)
            {
                if (input == 0.0f)
                {
                    return 0.0f;
                }
                else
                {
                    return 1.0f;
                }
            }
            else if(oneStepCurve)
            {
                if (input == 1.0f)
                {
                    return 1.0f;
                }
                else
                {
                    return 0.0f;
                }
            }
            else
                return Mathf.Clamp01(curve.Evaluate(input));

        }
    }
}
