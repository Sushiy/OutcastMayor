using OutcastMayor.Interaction;
using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "InventorySpaceConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/InventorySpaceConsideration", order = 1)]
    public class InventorySpaceConsideration : Consideration
    {
        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            ItemStackInstance itemStack = considerationData.data[0] as ItemStackInstance;
            Inventory inventory = controller.GetComponent<Inventory>();
            return Evaluate((float)inventory.CalculateSpaceFor(itemStack.source.item) / (float)itemStack.source.item.stackLimit);
        }
        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            ItemStackInstance itemStack = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is ItemStackInstance)
                {
                    itemStack = instanceData[i] as ItemStackInstance;
                }
            }
            considerationData = new ConsiderationData(this, new Object[] { itemStack });

            return itemStack != null;
        }
    }
}