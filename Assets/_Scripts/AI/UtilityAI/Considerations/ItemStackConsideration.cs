using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "HasItemStackConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/HasItemStackConsideration", order = 1)]
    public class ItemStackConsideration : Consideration
    {
        Inventory.ItemStack stack;

        public override float ScoreConsideration(UtilityAIController controller, ConsiderationData considerationData)
        {
            //!!! TODO: How do I get arbitrary info into the considerations? weird!
            //stack = action as

            Inventory i = controller.GetComponent<Inventory>();
            if(i.Contains(stack))
            {
                return Evaluate(1.0f);
            }
            return Evaluate(0.0f);
        }
        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);

            return true;
        }
    }
}
