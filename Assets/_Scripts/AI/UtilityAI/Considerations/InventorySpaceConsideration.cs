using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "InventorySpaceConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/InventorySpaceConsideration", order = 1)]
    public class InventorySpaceConsideration : Consideration
    {
        public override float ScoreConsideration(Action action, UtilityAIController controller, Object[] instanceData)
        {
            ItemStackInstance itemStack = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is ItemStackInstance)
                {
                    itemStack = instanceData[i] as ItemStackInstance;
                }
            }
            Inventory inventory = controller.GetComponent<Inventory>();
            return Evaluate((float)inventory.CalculateSpaceFor(itemStack.source.item) / (float)itemStack.source.item.stackLimit);
        }
    }
}