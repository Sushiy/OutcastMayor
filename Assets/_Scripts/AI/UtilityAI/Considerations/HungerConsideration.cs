using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "HungerConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/HungerConsideration", order = 1)]
    public class HungerConsideration : Consideration
    {
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[0];
        }
        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            return Evaluate(controller.satedness/maxValue);
        }

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);

            return true;
        }
    }
}
