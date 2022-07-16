using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "DistanceConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/DistanceConsideration", order = 1)]
    public class DistanceConsideration : Consideration
    {
        /// <summary>
        /// The distance is normalized to [0,normalizeCeiling]
        /// </summary>
        public float normalizeCeiling = 30.0f;

        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[] { typeof(Transform) };
        }

        public override string[] GetSourceValueNames()
        {
            return new string[] {"Distance to Target"};
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData data)
        {
            Transform targetTransform = data.data[0] as Transform;
            float distance = Vector3.Distance(controller.transform.position, targetTransform.position) / normalizeCeiling;

            return new float[] { distance};
        }

        protected override float CalculateScore(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Transform targetTransform = considerationData.data[0] as Transform;
            float distance = Vector3.Distance(controller.transform.position, targetTransform.position) / normalizeCeiling;

            return distance;
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
