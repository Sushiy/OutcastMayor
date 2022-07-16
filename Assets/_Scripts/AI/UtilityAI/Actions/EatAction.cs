using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "EatAction", menuName = "ScriptableObjects/UtilityAI/Actions/EatAction", order = 1)]
    public class EatAction : Action
    {

        public override void Init(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            controller.ChangeState(controller.PerformingState);
        }

        public override void Perform(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            Items.Food food = null;
            if (instanceData[0] is Items.Food)
            {
                food = instanceData[0] as Items.Food;
            }
            if(controller.Inventory.Contains(food))
            {
                controller.Eat(food);
                Debug.Log("<color=green>" + controller.name + " -> EatAction completed with " + food.DisplayName + "</color>");
                controller.ActionCompleted();
            }
            else
            {
                Debug.Log("<color=red>" + controller.name + " -> EatAction failed with " + food.DisplayName + "</color>");
                Cancel(controller, instanceData, instanceValues);
            }

        }
        public override void Cancel(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
            //Put away the food?
            controller.ActionCompleted();
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            Inventory inventory = controller.Inventory;
            if (inventory != null && inventory.slots.Length > 0)
            {
                for (int i = 0; i < inventory.slots.Length; i++)
                {
                    Inventory.ItemStack stack = inventory.slots[i];
                    if (stack.count > 0 && stack.item.HasTag(Item.Tag.Food) && stack.item is Items.Food)
                    {
                        Items.Food food = stack.item as Items.Food;
                        ActionInstance instance = new ActionInstance(this, owner, new Object[] { food, inventory }, new int[0]);
                        //Check if the instance is valid before adding it to the list
                        if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                        {
                            instances.Add(instance);
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
        public override bool CheckInstanceRequirement(SmartObject owner, Object[] instanceData, int[] instanceValues)
        {
            Inventory inventory = null;
            Items.Food food = null;

            if (instanceData[1] is Inventory)
            {
                inventory = instanceData[1] as Inventory;
            }
            else
            {
                Debug.LogError("No Inventory");
                return false;
            }            

            if (instanceData[0] is Items.Food)
            {
                food = instanceData[0] as Items.Food;
                return inventory.Contains(food);
            }
            else
            {
                Debug.LogError("No Food");
                return false;
            }
        }

        public override System.Type[] GetProvidedDataTypes()
        {
            return new System.Type[] { typeof(Items.Food), typeof(Inventory) };
        }
    }
}
