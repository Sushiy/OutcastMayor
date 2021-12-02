using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "ConstructionsAvailableConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/ConstructionsAvailableConsideration", order = 1)]
    public class ConstructionsAvailableConsideration : Consideration
    {
        public override float ScoreConsideration(Action action, UtilityAIController controller, Object[] instanceData)
        {
            if(controller.availableConstructions.Count > 0)
            {
                return Evaluate(1.0f);
            }
            else
            {
                return Evaluate(0.0f);
            }
        }
    }
}
