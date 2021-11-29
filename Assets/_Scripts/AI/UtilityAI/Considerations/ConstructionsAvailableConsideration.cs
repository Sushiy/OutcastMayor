using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "ConstructionsAvailableConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/ConstructionsAvailableConsideration", order = 1)]
    public class ConstructionsAvailableConsideration : Consideration
    {
        public override float ScoreConsideration(Action action, UtilityAIController controller)
        {
            if(controller.availableConstructions.Count > 0)
            {
                return curve.Evaluate(1.0f);
            }
            else
            {
                return curve.Evaluate(0.0f);
            }
        }
    }
}
