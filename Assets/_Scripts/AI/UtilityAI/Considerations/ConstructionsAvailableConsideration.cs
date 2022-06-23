using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "ConstructionsAvailableConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/ConstructionsAvailableConsideration", order = 1)]
    public class ConstructionsAvailableConsideration : Consideration
    {
        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
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

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);
            return true;
        }
    }
}
