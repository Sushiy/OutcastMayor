using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "HasFoodConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/HasFoodConsideration", order = 1)]
    public class HasFoodConsideration : Consideration
    {
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[0];
        }

        public override string[] GetSourceValueNames()
        {
            return new string[] { "Has Food in Inventory" };
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Inventory inventory = controller.Inventory;

            if (inventory.Contains(Item.Tag.Food))
            {
                return new float[] { 1.0f};
            }
            else
            {
                return new float[] { 0.0f };
            }
        }

        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Inventory inventory = controller.Inventory;

            if(inventory.Contains(Item.Tag.Food))
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