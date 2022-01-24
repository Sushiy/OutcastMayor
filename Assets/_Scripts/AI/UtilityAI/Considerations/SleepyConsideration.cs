using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "SleepyConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/SleepyConsideration", order = 1)]
    public class SleepyConsideration : Consideration
    {
        public override float ScoreConsideration(UtilityAIController controller, ConsiderationData considerationData)
        {
            return Evaluate(controller.sleepy);
        }
        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);

            return true;
        }
    }
}
