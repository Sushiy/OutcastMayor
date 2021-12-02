using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "DistanceConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/DistanceConsideration", order = 1)]
    public class DistanceConsideration : Consideration
    {
        public override float ScoreConsideration(Action action, UtilityAIController controller, Object[] instanceData)
        {
            Transform targetTransform = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Transform)
                {
                    targetTransform = instanceData[i] as Transform;
                }
            }
            float distance = Vector3.Distance(controller.transform.position, targetTransform.position) / maxValue;

            return Evaluate(distance);
        }
    }
}
