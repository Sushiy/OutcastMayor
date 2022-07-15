using OutcastMayor.Interaction;
using OutcastMayor.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "InventorySpaceConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/InventorySpaceConsideration", order = 1)]
    public class InventorySpaceConsideration : Consideration
    {
        public override bool HasAllData(Type[] actionData)
        {
            for (int j = 0; j < actionData.Length; j++)
            {
                if (actionData[j] == typeof(Item) || actionData[j].IsSubclassOf(typeof(Item)))
                {
                    return true;
                }
                if (actionData[j] == typeof(ItemStackInstance) || actionData[j].IsSubclassOf(typeof(ItemStackInstance)))
                {
                    return true;
                }
            }
            return false;
        }
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[] { typeof(Item), typeof(ItemStackInstance) };
        }
        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Item item = considerationData.data[0] as Item;
            Inventory inventory = controller.GetComponent<Inventory>();
            return Evaluate((float)inventory.CalculateSpaceFor(item) / (float)item.stackLimit);
        }
        public override bool TryGetConsiderationData(UnityEngine.Object[] instanceData, out ConsiderationData considerationData)
        {
            ItemStackInstance itemStack = null;
            Item item = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is ItemStackInstance)
                {
                    itemStack = instanceData[i] as ItemStackInstance;
                    break;
                }
                if(instanceData[i] is Item)
                {
                    item = instanceData[i] as Item;
                    break;
                }
            }

            if(itemStack != null)
            {
                considerationData = new ConsiderationData(this, new UnityEngine.Object[] { itemStack.source.item });
                return true;
            }
            else
            {
                considerationData = new ConsiderationData(this, new UnityEngine.Object[] { item });
                return item != null;
            }

        }

        public override string[] GetSourceValueNames()
        {
            return new string[] { "Space for how many stacks" };
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Item item = considerationData.data[0] as Item;
            Inventory inventory = controller.GetComponent<Inventory>();
            return new float[] { (float)inventory.CalculateSpaceFor(item) / (float)item.stackLimit };
        }
    }
}