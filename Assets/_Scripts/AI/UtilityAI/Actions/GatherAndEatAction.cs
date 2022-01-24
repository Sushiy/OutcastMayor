using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "GatherAndEatAction", menuName = "ScriptableObjects/UtilityAI/Actions/GatherAndEatAction", order = 1)]
    public class GatherAndEatAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " went to eat");

            ItemStackInstance itemStack = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is ItemStackInstance)
                {
                    itemStack = instanceData[i] as ItemStackInstance;
                }
            }
            if (itemStack == null)
            {
                Debug.LogError("No itemStack");
                return;
            }

            Vector3 target = itemStack.transform.position + (controller.transform.position - itemStack.transform.position).normalized * 1.0f;
            controller.aIMovement.MoveTo(target, false, () => CompleteAction(itemStack, controller));
        }

        public void CompleteAction(ItemStackInstance itemStack, UtilityAIController controller)
        {
            itemStack.Interact(controller.Interactor);
            string log = controller.name + " -> GatherAndEatAction \n";

            if(controller.Inventory.Contains("Apple"))
            {
                controller.Inventory.Delete("Apple");
            }
            controller.food += .5f;
            Debug.Log(log);
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            ItemStackInstance itemStackInstance = owner.GetComponent<ItemStackInstance>();

            if (itemStackInstance != null)
            {

                for (int i = 0; i < controller.availableStockpiles.Count; i++)
                {
                    ActionInstance instance = new ActionInstance(this, owner, new Object[] { itemStackInstance, owner.transform }, new int[0]);
                    if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                    {
                        instances.Add(instance);
                    }
                }

            }
            return instances.ToArray();
        }
    }
}
