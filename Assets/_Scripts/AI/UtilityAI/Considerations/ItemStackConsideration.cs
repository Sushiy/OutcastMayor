using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "HasItemStackConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/HasItemStackConsideration", order = 1)]
    public class ItemStackConsideration : Consideration
    {
        public Inventory.ItemStack stack;

        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[0];
        }

        public override string[] GetSourceValueNames()
        {
            return new string[] {"Has Itemstack"};
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Inventory i = controller.GetComponent<Inventory>();
            if (i.Contains(stack))
            {
                return new float[] { 1.0f };
            }
            return new float[] { 0.0f };
        }

        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
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
