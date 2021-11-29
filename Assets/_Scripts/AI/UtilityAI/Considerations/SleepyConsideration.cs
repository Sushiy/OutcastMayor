using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "SleepyConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/SleepyConsideration", order = 1)]
    public class SleepyConsideration : Consideration
    {
        public override float ScoreConsideration(Action action, UtilityAIController controller)
        {
            return curve.Evaluate(controller.sleepy);
        }
    }
}
