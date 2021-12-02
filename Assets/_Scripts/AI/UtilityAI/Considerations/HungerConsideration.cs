using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "HungerConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/HungerConsideration", order = 1)]
    public class HungerConsideration : Consideration
    {
        public override float ScoreConsideration(Action action, UtilityAIController controller, Object[] instanceData)
        {
            return Evaluate(controller.food/maxValue);
        }
    }
}
