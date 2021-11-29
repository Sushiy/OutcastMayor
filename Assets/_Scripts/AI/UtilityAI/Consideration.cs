using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public abstract class Consideration : ScriptableObject
    {
        public string Name;
        [SerializeField] protected AnimationCurve curve;

        [SerializeField] protected float maxValue = 1.0f;

        /// <summary>
        /// An exclusive consideration HAS to be greater than 0 so that the action in whole is valid
        /// </summary>
        public bool isExclusiveConsideration = false;
        public abstract float ScoreConsideration(Action action, UtilityAIController controller);
    }
}
