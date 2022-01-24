using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "HasFoodConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/HasFoodConsideration", order = 1)]
    public class HasFoodConsideration : Consideration
    {
        public override float ScoreConsideration(UtilityAIController controller, ConsiderationData considerationData)
        {
            Inventory inventory = controller.Inventory;

            if(inventory.Contains("Apple"))
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);

            return true;
        }
    }
}