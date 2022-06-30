using OutcastMayor.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "GetFoodAction", menuName = "ScriptableObjects/UtilityAI/Actions/GetFoodAction", order = 1)]
    public class GetFoodAction : Action
    {
        public override void Init(UtilityAICharacter controller, UnityEngine.Object[] instanceData, int[] instanceValues)
        {
            //Do stuff once at the beginning
            Transform moveTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Transform)
                {
                    moveTarget = instanceData[i] as Transform;
                }
            }

            Vector3 target = moveTarget.transform.position + (controller.transform.position - moveTarget.transform.position).normalized * 1.0f;

            Debug.Log("<color=green>" + controller.name + "-> GetFoodAction moveto.</color>");

            controller.MoveTo(target, false);
        }

        public override void Perform(UtilityAICharacter controller, UnityEngine.Object[] instanceData, int[] instanceValues)
        {
            Item itemTarget = null;
            Stockpile stockpileTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Item)
                {
                    itemTarget = instanceData[i] as Item;
                }
                if (instanceData[i] is Stockpile)
                {
                    stockpileTarget = instanceData[i] as Stockpile;
                }
            }
            if (itemTarget == null)
            {
                Debug.LogError("No Item");
                return;
            }
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return;
            }

            if(stockpileTarget.inventory.Contains(itemTarget))
            {
                stockpileTarget.inventory.Delete(itemTarget, 1);
                controller.Inventory.Add(new Inventory.ItemStack(itemTarget, 1));
                Debug.Log("<color=green>" + controller.name + " -> GetFoodAction completed with " + itemTarget.DisplayName + "</color>");
                controller.ActionCompleted();
            }
            else
            {
                Debug.Log("<color=red>" + controller.name + " -> GetFoodAction failed with " + itemTarget.DisplayName + "</color>");
                Cancel(controller, instanceData, instanceValues);
            }
        }
        public override void Cancel(UtilityAICharacter controller, UnityEngine.Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
            controller.ActionCompleted();
        }

        /// <summary>
        /// Build Instance of this Action. GetFoodActions will generate one instance for each food item available in the inventory
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            Stockpile stockpile = owner.GetComponent<Stockpile>();
            if(stockpile != null)
            {
                Inventory inventory = stockpile.inventory;
                if (inventory != null && inventory.slots.Length > 0)
                {
                    for (int i = 0; i < inventory.slots.Length; i++)
                    {
                        Inventory.ItemStack stack = inventory.slots[i];
                        if (stack.count > 0 && stack.item.HasTag(Item.Tag.Food))
                        {
                            ActionInstance instance = new ActionInstance(this, owner, new UnityEngine.Object[] { stack.item, stockpile, owner.transform }, new int[0]);
                            //Check if the instance is valid before adding it to the list
                            if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                            {
                                instances.Add(instance);
                            }
                        }
                    }
                }

            }

            return instances.ToArray();
        }
        /// <summary>
        /// Verify if this instance has everything is valid to be checked
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="instanceData"></param>
        /// <param name="instanceValues"></param>
        /// <returns></returns>
        public override bool CheckInstanceRequirement(SmartObject owner, UnityEngine.Object[] instanceData, int[] instanceValues)
        {
            if (owner.isOccupied) return false;
            Item item = null;
            Stockpile stockpileTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Item)
                {
                    item = instanceData[i] as Item;
                }
                if (instanceData[i] is Stockpile)
                {
                    stockpileTarget = instanceData[i] as Stockpile;
                }
            }
            if(item == null)
            {
                Debug.LogError("No Item");
                return false;
            }
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return false;
            }
            return true;
        }

        public override Type[] GetProvidedDataTypes()
        {
            return new Type[] { typeof(Item), typeof(Stockpile), typeof(Transform)};
        }
    }

}
