using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "HungerConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/HungerConsideration", order = 1)]
    public class HungerConsideration : Consideration
    {
        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            return Evaluate(controller.food/maxValue);
        }

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);

            return true;
        }
    }
}
