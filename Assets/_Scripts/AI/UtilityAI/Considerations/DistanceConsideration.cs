using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "DistanceConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/DistanceConsideration", order = 1)]
    public class DistanceConsideration : Consideration
    {
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[] { typeof(Transform) };
        }
        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Transform targetTransform = considerationData.data[0] as Transform;
            float distance = Vector3.Distance(controller.transform.position, targetTransform.position) / maxValue;

            return Evaluate(distance);
        }

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            Transform targetTransform = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Transform)
                {
                    targetTransform = instanceData[i] as Transform;
                    break;
                }
            }
            considerationData = new ConsiderationData(this, new Object[] { targetTransform });
            return targetTransform != null;
        }
    }
}
